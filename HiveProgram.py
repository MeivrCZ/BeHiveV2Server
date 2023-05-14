import serial
import time
import json
import requests
from picamera import PiCamera
import base64
import numpy as np
import os
import sqlalchemy
from sqlalchemy_utils import database_exists, create_database
from sqlalchemy import create_engine, Column, Integer, String
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
import threading
import keyboard
import tkinter as tk
from tkinter import ttk
import sys
from PIL import Image, ImageTk
import psutil


backgroundColor = '#2E3E64'

ServerIP = '192.168.0.6'
ServerAddress = 'https://192.168.0.6:443/'
TestPath = 'restapi/testAction'
PostPath = 'restapi/SHB1PostDataJson'
PostImgAddress = 'restapi/SHB1PostImageJson'

Id = 2
Serial = "156188"

photo = ""
session = ""

#post delays
cameraDelay = 5
dataGatherDelay = 30
dataPostDelay = 120

#config
configFile = 'config.json'

def readConfig():
    global ServerAddress
    global cameraDelay
    global dataGatherDelay
    global dataPostDelay
    global Id
    global Serial
    with open(configFile, 'r') as f:
        data = json.load(f)
        ServerAddress = data["ServerAddress"]
        Id = data["Id"]
        Serial = data["Serial"]
        cameraDelay = data["cameraDelay"]
        dataGatherDelay = data["dataGatherDelay"]
        dataPostDelay = data["dataPostDelay"]


def deleteConfigContent():
    # Delete the contents of the JSON file
    with open(configFile, 'w') as f:
        json.dump({}, f)

def storeConfigData():
    data = {"ServerAddress": ServerAddress,
            "Id": Id,
            "Serial": Serial,
            "cameraDelay": cameraDelay,
            "dataGatherDelay": dataGatherDelay,
            "dataPostDelay": dataPostDelay}
    with open(configFile, 'w') as f:
        json.dump(data, f)


if not os.path.isfile(configFile):
    deleteConfigContent()
    storeConfigData()
else:
    readConfig()

#models

class Data:
    def __init__(self, outsideTemperature, pressure, weight, humidity, unixTimestamp, insideTemperature):
        self.outsideTemperature = outsideTemperature
        self.pressure = pressure
        self.humidity = humidity
        self.weight = weight
        self.unixTimestamp = unixTimestamp
        self.insideTemperature = insideTemperature

#arduino connection and gathering

class Arduino:
    def getData(self):
        print("gathering of data started")
        ser.write("ACTION 1;".encode('utf-8'))
        print("message writen")
        msg = ""

        isavable = False
        print("waiting for data")
        for i in range(0, 5):
            if(ser.in_waiting > 0):
                isavable = True
                break
            time.sleep(1)
        if(isavable == False):
            print("requert timed out")
            return -1

        print("reading message")

        msg = ser.read_until(';')
        msg = msg.decode('utf-8')
        msg = msg.replace(';', '')

        print("message aquired")

        return str(msg)

#data posting

class DataUploadService:
    header = {'Content-type': 'application/json', 'Accept': 'text/plain'}

    def testInternetConnection(self):
        try:
            response = requests.get("https://www.google.com", timeout=2)
            response.raise_for_status()
            return True
        except requests.exceptions.RequestException:
            return False

    def testServerConnection(self):
        try:
            response = requests.get(ServerAddress, verify=False, timeout=2)
            response.raise_for_status()
            return True
        except requests.exceptions.RequestException:
            return False

    def postData(self, address, data, id, serialCode):
        print("data post started")

        #print json in cmd
        jlist = {
            'Id': int(Id),
            'Serial': Serial,
            'unixTimestamp': int(data.unixTimestamp),
            'outsideTemperature': float(data.outsideTemperature),
            'pressure': float(data.pressure),
            'humidity': float(data.humidity),
            'weight': float(data.weight),
            'insideTemperature': float(data.insideTemperature)
        }

        jasus = json.dumps(jlist)

        print(jasus)

        print(address)
        print(id)
        print(serialCode)

        #post json

        try:
            print("post request sending started")
            response = requests.post(address, verify=False, json={
                                                    'Id': int(id),
                                                    'Serial': serialCode,
                                                    'unixTimestamp': int(data.unixTimestamp),
                                                    'outsideTemperature': float(data.outsideTemperature),
                                                    'pressure': float(data.pressure),
                                                    'humidity': float(data.humidity),
                                                    'weight': float(data.weight),
                                                    'insideTemperature': float(data.insideTemperature)})

            if (response.status_code != 200):
                print("ERROR - " + str(response.status_code))
                return -1

            print("data succsesfuly sended")
            return str(response.content.decode("utf-8"))

        except Exception as e:
            print("ERROR - error while posting has ocured")
            print(e)
            return -1

        return -1


    def postImg(self, address, id, serialCode, img):
        print("posting image started")
        try:
            response = requests.post(address, verify=False, json={'Id': int(id),
                                                    'Serial': str(serialCode),
                                                    'Image': str(img)})
        except Exception as e:
            print("error while posting image has ocured")
            print(e)
            return "-1"

        if (response.status_code != 200):
            print("error" + str(response.status_code))
            return "-1"

        print("image succsesfuly sended")
        return str(response.content.decode("utf-8"))

#GUI
class RedirectText(object):
    def __init__(self, text_widget):
        self.text_widget = text_widget

    def write(self, string):
        self.text_widget.insert("end", string)
        self.text_widget.see("end")

class RootFrame(tk.Frame):
    def __init__(self, master=None):
        super().__init__(master)
        self.master = master
        self.pack()
        self.create_widgets()

    def create_widgets(self):
        self.label = tk.Label(self, text="BeHive SHB1", font=("Arial", 24), foreground='white', background=backgroundColor)
        self.label.pack()

        self.settings_button = tk.Button(self, text="Settings", command=self.show_settings_frame)
        self.settings_button.pack()

        self.camera_button = tk.Button(self, text="Camera", command=self.show_camera_frame)
        self.camera_button.pack()

        self.data_button = tk.Button(self, text="Data", command=self.show_data_frame)
        self.data_button.pack()

        self.terminal_button = tk.Button(self, text="Terminal", command=self.show_terminal_frame)
        self.terminal_button.pack()

    def show_settings_frame(self):
        self.hide_widgets()
        self.settings_frame = SettingsFrame(self)
        self.settings_frame.pack()

    def show_camera_frame(self):
        self.hide_widgets()
        self.camera_frame = CameraFrame(self)
        self.camera_frame.pack()

    def show_data_frame(self):
        self.hide_widgets()
        self.data_frame = DataFrame(self)
        self.data_frame.pack()

    def show_terminal_frame(self):
        self.hide_widgets()
        self.terminal_frame = TerminalFrame(self)
        self.terminal_frame.pack()

    def hide_widgets(self):
        self.label.pack_forget()
        self.settings_button.pack_forget()
        self.camera_button.pack_forget()
        self.data_button.pack_forget()
        self.terminal_button.pack_forget()

class SettingsFrame(tk.Frame):
    def __init__(self, master=None):
        super().__init__(master)
        self.master = master
        self.pack()
        self.create_widgets()

    def save(self):
        global ServerAddress
        global cameraDelay
        global dataGatherDelay
        global dataPostDelay
        global Id
        global Serial

        ServerAddress = self.serverAddressEntery.get()
        cameraDelay = int(self.cameraTimeBox.get())
        dataGatherDelay = int(self.dataCapBox.get())
        dataPostDelay = int(self.dataSendBox.get())
        Id = int(self.idEntery.get())
        Serial = self.serialEntery.get()

        storeConfigData()

    def create_widgets(self):
        global ServerAddress
        global cameraDelay
        global dataGatherDelay
        global dataPostDelay
        global Id
        global Serial

        self.back_button = tk.Button(self, text="Back", command=self.go_back)
        self.Name = tk.Label(self, text="Settings")
        self.serverAddressLabel = tk.Label(self, text="Server Address:")
        self.serverAddressEntery = tk.Entry(self)
        self.cameraCheck = tk.Checkbutton(self, text="Use camera", state="disabled", variable=True)
        self.cameraTimeBox = tk.Spinbox(self,  increment=1)
        self.cameraTimeLabel = tk.Label(self, text="cap. time (sec)")
        self.dataCapNameLabel = tk.Label(self, text="Data cap. time")
        self.dataCapBox = tk.Spinbox(self, increment=5)
        self.dataCapTimeLabel = tk.Label(self, text="(sec)")
        self.dataSendNameLabel = tk.Label(self, text="Data send time")
        self.dataSendBox = tk.Spinbox(self, increment=60)
        self.dataSendTimeLabel = tk.Label(self, text="(sec)")
        self.idLabel = tk.Label(self, text="Id:")
        self.idEntery = tk.Entry(self)
        self.serialLabel = tk.Label(self, text="Serial:")
        self.serialEntery = tk.Entry(self)
        self.save_button = tk.Button(self, text="Save", command=self.save)

        self.serverAddressEntery.insert(0, ServerAddress)
        self.cameraTimeBox.insert(0, cameraDelay)
        self.dataCapBox.insert(0, dataGatherDelay)
        self.dataSendBox.insert(0, dataPostDelay)
        self.idEntery.insert(0, Id)
        self.serialEntery.insert(0, Serial)

        self.back_button.grid(row=0, column=0, sticky="new")
        self.Name.grid(row=0, column=1, columnspan=3, sticky="new")
        self.serverAddressLabel.grid(row=1, column=1, sticky="new")
        self.serverAddressEntery.grid(row=1, column=2, columnspan=2, sticky="new")
        self.cameraCheck.grid(row=2, column=1, sticky="new")
        self.cameraTimeBox.grid(row=2, column=2, sticky="new")
        self.cameraTimeLabel.grid(row=2, column=3, sticky="new")
        self.dataCapNameLabel.grid(row=3, column=1, sticky="new")
        self.dataCapBox.grid(row=3, column=2, sticky="new")
        self.dataCapTimeLabel.grid(row=3, column=3, sticky="new")
        self.dataSendNameLabel.grid(row=4, column=1, sticky="new")
        self.dataSendBox.grid(row=4, column=2, sticky="new")
        self.dataSendTimeLabel.grid(row=4, column=3, sticky="new")
        self.idLabel.grid(row=5, column=1, sticky="new")
        self.idEntery.grid(row=5, column=2, sticky="new")
        self.serialLabel.grid(row=6, column=1, sticky="new")
        self.serialEntery.grid(row=6, column=2, columnspan=2, sticky="new")
        self.save_button.grid(row=7, column=1, columnspan=3, sticky="new")

        self.columnconfigure(0, minsize=25, weight=1)
        self.columnconfigure(1, minsize=83, weight=2)
        self.columnconfigure(2, minsize=84, weight=2)
        self.columnconfigure(3, minsize=83, weight=2)
        self.columnconfigure(4, minsize=25, weight=1)



    def go_back(self):
        self.pack_forget()
        self.master.create_widgets()
        self.master.pack()

class CameraFrame(tk.Frame):
    def __init__(self, master=None):
        super().__init__(master)
        self.master = master
        self.pack()
        self.create_widgets()

    def update_image(self):
        global photo
        try:
            while self.cameraRun:
                self.imageLabel.configure(image=photo)
                self.imageLabel.image = photo

                time.sleep(1) #0.03
        except Exception as e:
            print("error, no image to be displayed")
            print(e)


    def create_widgets(self):
        global photo
        self.back_button = tk.Button(self, text="Back", command=self.go_back)
        self.Name = tk.Label(self, text="Camera")
        self.imageLabel = tk.Label(self)

        self.back_button.grid(row=0, column=0, sticky="new")
        self.Name.grid(row=0, column=1, columnspan=3, sticky="new")
        self.imageLabel.grid(row=1, column=1, columnspan=3, sticky="nsew")

        try:
            self.imageLabel.configure(image=photo)
            self.imageLabel.image = photo
        except Exception as e:
            print("error, no image to be displayed")
            print(e)

        self.columnconfigure(0, minsize=45, weight=1)
        self.columnconfigure(1, minsize=63, weight=2)
        self.columnconfigure(2, minsize=84, weight=2)
        self.columnconfigure(3, minsize=83, weight=2)
        self.columnconfigure(4, minsize=25, weight=1)

        self.cameraRun = True
        self.photoUpdateThread = threading.Thread(target=self.update_image)
        self.photoUpdateThread.start()

    def go_back(self):
        self.cameraRun = False
        self.pack_forget()
        self.master.create_widgets()
        self.master.pack()

class DataFrame(tk.Frame):
    def __init__(self, master=None):
        super().__init__(master)
        self.master = master
        self.pack()
        self.create_widgets()

    def update_table(self):
        global session
        # clear the table
        self.table.delete(*self.table.get_children())
        # add new rows to the table
        all_data = session.query(HiveData).all()
        count = 0
        for data in all_data:
            stuff = convertData(data.data)
            self.table.insert('', str(count), text=str(stuff.unixTimestamp), values=(stuff.insideTemperature, stuff.outsideTemperature, stuff.pressure, stuff.humidity, stuff.weight))
            count = count + 1


    def create_widgets(self):
        self.back_button = tk.Button(self, text="Back", command=self.go_back)
        self.Name = tk.Label(self, text="Data")
        self.scrollbar = tk.Scrollbar(self)
        self.table = ttk.Treeview(self, yscrollcommand=self.scrollbar.set)
        self.button = tk.Button(self, text='Update Table', command=self.update_table)

        self.table['columns'] = ('insideTemp', 'outsideTemp', 'pressure', 'humidity', 'weight')
        self.table.column('#0', width=110)
        self.table.column('insideTemp', width=45)
        self.table.column('outsideTemp', width=45)
        self.table.column('humidity', width=25)
        self.table.column('weight', width=50)
        self.table.column('pressure', width=50)
        self.table.heading('#0', text='UTime')
        self.table.heading('insideTemp', text='ITmp')
        self.table.heading('outsideTemp', text='OTmp')
        self.table.heading('humidity', text='Humi')
        self.table.heading('weight', text='Weig')
        self.table.heading('pressure', text='Pres')

        # self.table.config(height=(self.table.winfo_screenheight() - 64) // 14)
        self.scrollbar.config(command=self.table.yview)

        self.back_button.grid(row=0, column=0, sticky="new")
        self.Name.grid(row=0, column=1, columnspan=3, sticky="new")
        self.table.grid(row=1, column=1, columnspan=3, sticky="nsew")
        self.scrollbar.grid(row=1, column=4, sticky="nsew")
        self.button.grid(row=2, column=1, columnspan=3, sticky="new")

        self.columnconfigure(0, minsize=25, weight=1)
        self.columnconfigure(1, minsize=83, weight=2)
        self.columnconfigure(2, minsize=84, weight=2)
        self.columnconfigure(3, minsize=83, weight=2)
        self.columnconfigure(4, minsize=25, weight=1)

    def go_back(self):
        self.pack_forget()
        self.master.create_widgets()
        self.master.pack()

class TerminalFrame(tk.Frame):
    def __init__(self, master=None):
        super().__init__(master)
        self.master = master
        self.pack()
        self.create_widgets()

    def create_widgets(self):
        self.back_button = tk.Button(self, text="Back", command=self.go_back)
        self.Name = tk.Label(self, text="Terminal")
        self.scrollbar = tk.Scrollbar(self)
        self.text_widget = tk.Text(self, yscrollcommand=self.scrollbar.set)

        self.back_button.grid(row=0, column=0, sticky="new")
        self.Name.grid(row=0, column=1, columnspan=3, sticky="new")
        self.text_widget.grid(row=1, column=1, columnspan=3, sticky="new")
        self.scrollbar.grid(row=1, column=4, sticky="nsew")

        self.text_widget.config(wrap="word")
        self.text_widget.config(height=(self.text_widget.winfo_screenheight() - 150) // 14)
        self.scrollbar.config(command=self.text_widget.yview)
        self.text_widget.see("end")

        self.columnconfigure(0, minsize=45, weight=1)
        self.columnconfigure(1, minsize=63, weight=2)
        self.columnconfigure(2, minsize=84, weight=2)
        self.columnconfigure(3, minsize=83, weight=2)
        self.columnconfigure(4, minsize=25, weight=1)

        sys.stdout = RedirectText(self.text_widget)

    def go_back(self):
        self.pack_forget()
        self.master.create_widgets()
        self.master.pack()


#database
#string: 'Server=127.0.0.1;Port=5432;Database=HiveDB;User Id=vojta;Password=1234;'
engine = create_engine('postgresql://vojta:1234@127.0.0.1:5432/HiveDB', echo = True)
if not database_exists(engine.url):
    create_database(engine.url)

# Deklarace základní třídy pro ORM
Base = declarative_base()

# Definice třídy pro tabulku 'hivedata'
class HiveData(Base):
    __tablename__ = 'hivedata'
    id = Column(Integer, primary_key=True)
    data = Column(String)

# Vytvoření tabulky v databázi
Base.metadata.create_all(engine)

# Vytvoření spojení s databází pomocí ORM session
Session = sessionmaker(bind=engine)
session = Session()


#serial
ser = serial.Serial('/dev/ttyS0', 115200, timeout=10)
ser.reset_input_buffer()
ard = Arduino()

#data collections
dataList = []

#data posting
uploadService = DataUploadService()

#camera
camera = PiCamera()
camera.rotation = 180
camera.resolution = (1920, 1080)
camera.framerate = 24
time.sleep(2)

#data gathering
def gatherData():
    try:
        gatheredjson = ard.getData()
        if (gatheredjson == -1):
            print("gathering failed")
            return

        data = HiveData(data=gatheredjson)

        ardJson = json.loads(data.data)
        # outsideTemperature, pressure, weight, humidity, unixTimestamp, insideTemperature
        info = Data(ardJson["D"], ardJson["P"], ardJson["W"], ardJson["H"], time.time(), ardJson["T"])

        info = {
            'U': int(info.unixTimestamp),
            'D': float(info.outsideTemperature),
            'P': float(info.pressure),
            'H': float(info.humidity),
            'W': float(info.weight),
            'T': float(info.insideTemperature)
        }
        data = HiveData(data=json.dumps(info))
        session.add(data)
        session.commit()
        print("gathering succesful")
    except Exception as e:
        print("error while saving data to database")
        print(e)
        return

def convertData(data):
    print("converting data")
    try:
        ardJson = json.loads(data)
        # outsideTemperature, pressure, weight, humidity, unixTimestamp, insideTemperature
        info = Data(ardJson["D"], ardJson["P"], ardJson["W"], ardJson["H"], ardJson["U"], ardJson["T"])

        return info
    except:
        print("error while converting ocured")
        return


#tasks

def PostCameraTask():
    global photo
    while(True):
        camera.capture('image.jpg')
        img = Image.open("image.jpg")
        img = img.resize((460, 252))
        photo = ImageTk.PhotoImage(img)
        image = ""
        with open('image.jpg', "rb") as img_file:
            image = base64.b64encode(img_file.read()).decode('utf-8')
        if (uploadService.testInternetConnection()):
            if (uploadService.testServerConnection()):
                uploadService.postImg(ServerAddress + PostImgAddress, Id, Serial, image)
            else:
                print("ERROR - server is not available")
        else:
            print("ERROR - no internet connection")
        time.sleep(cameraDelay)

def GatherDataTask():
    while(True):
        gatherData()
        time.sleep(dataGatherDelay)

def PostDataTask():
    while(True):
        if (uploadService.testInternetConnection()):
            if (uploadService.testServerConnection()):
                all_data = session.query(HiveData).all()
                for data in all_data:
                    response = uploadService.postData(ServerAddress + PostPath, convertData(data.data), Id, Serial)
                    if (response != -1):
                        session.query(HiveData).filter_by(id=data.id).delete()
                        session.commit()
                cicle = 1
            else:
                print("ERROR - server is not available")
        else:
            print("ERROR - no internet connection")
        time.sleep(dataPostDelay)


# --------------program-----------------

#start tasks
#send image task
postCamera_Thread = threading.Thread(target=PostCameraTask)
postCamera_Thread.start()
#gather data task
gatherData_Thread = threading.Thread(target=GatherDataTask)
gatherData_Thread.start()
#post data task
postData_Thread = threading.Thread(target=PostDataTask)
postData_Thread.start()

#Start GUI

root = tk.Tk()

root.title("BeHiveSHB1Software")
root.configure(background=backgroundColor)
root.attributes('-zoomed', True)

app = RootFrame(master=root)

app.mainloop()