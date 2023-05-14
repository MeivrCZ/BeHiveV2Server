#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BMP280.h>
#include <ArduinoJson.h>
#include "HX711.h"
#include "DHT.h"

//mic
const int mic = A0;

//dht11
#define DHTPIN 3
#define DHTTYPE DHT11   // DHT 11
DHT dht(DHTPIN, DHTTYPE);

//HX711
HX711 scale;
uint8_t Scale_dataPin = 11;
uint8_t Scale_clockPin = 10;

//BMP280
#define BMP280_I2C_ADDRESS  0x76
Adafruit_BMP280 bmp280;

int getWeight(){
  return scale.get_units(10);
}

long getLoud(){
  //1.
  int cikle = 0;
  //long normal = 0;
  long numofwaves = 0;
  bool isOn = false;
  while(cikle != 10000){
    if(digitalRead(mic) == 1 && isOn == false){
      isOn = true;
      numofwaves++;
    }
    if(digitalRead(mic) == 0){
      isOn = false;
    }
    delayMicroseconds(100);
    //normal += analogRead(mic);
    cikle++;
  }
  //return normal / 10000;
  return numofwaves;
}

float getTemperature(){
  return bmp280.readTemperature();
}

float getPressure(){
  return bmp280.readPressure()/100;
}

String getJson(){
  StaticJsonDocument<512> doc;
  doc["W"] = getWeight();
  
  doc["H"] = dht.readHumidity();
  doc["T"] = dht.readTemperature();
  doc["P"] = getPressure();
  doc["D"] = getTemperature();

  String JsonWithData = "";
  serializeJson(doc, JsonWithData);

  return JsonWithData;
}

void setup() {
  pinMode(mic, INPUT);
  
  Serial.begin(115200);
  
  dht.begin();
  bmp280.begin(BMP280_I2C_ADDRESS);
  scale.begin(Scale_dataPin, Scale_clockPin);
  scale.set_scale(20.84);
  scale.tare();
}

void loop() {
  if (Serial.available() > 0){
    String incomingRequest = Serial.readStringUntil(';');
    delay(100);
    if(incomingRequest == "ACTION 1"){
        String msg = getJson() + ";";
        Serial.print(msg);
      }
    }
}
