#include <ArduinoJson.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>

const int bomba1Pin = 2;
const int bomba2Pin = 3;
const int sensorFlujo1Pin = 4;
const int sensorFlujo2Pin = 5;

volatile uint16_t contadorPulsos1 = 0;
volatile uint16_t contadorPulsos2 = 0;

LiquidCrystal_I2C lcd(0x27, 16, 2); // Dirección I2C de la pantalla LCD, ajustar si es diferente

void contarPulsos1() {
    contadorPulsos1++;
}

void contarPulsos2() {
    contadorPulsos2++;
}

void setup() {
    Serial.begin(9600);

    pinMode(bomba1Pin, OUTPUT);
    pinMode(bomba2Pin, OUTPUT);
    pinMode(sensorFlujo1Pin, INPUT);
    pinMode(sensorFlujo2Pin, INPUT);

    attachInterrupt(digitalPinToInterrupt(sensorFlujo1Pin), contarPulsos1, RISING);
    attachInterrupt(digitalPinToInterrupt(sensorFlujo2Pin), contarPulsos2, RISING);

    lcd.init(); // Inicializa la LCD
    lcd.backlight(); // Enciende la luz de fondo
    lcd.print("Sistema Listo");
}

void loop() {
    if (Serial.available()) {
        String input = Serial.readString();
        DynamicJsonDocument doc(1024);
        DeserializationError error = deserializeJson(doc, input);

        if (error) {
            Serial.println("Error al parsear JSON");
            return;
        }

        const char* accion = doc["accion"];

        if (strcmp(accion, "activar") == 0) {
            int cliente = doc["cliente"];
            if (cliente == 1) {
                digitalWrite(bomba1Pin, HIGH);
                delay(5000); // Simula el tiempo de operación de la bomba
                digitalWrite(bomba1Pin, LOW);

                float litros = contadorPulsos1 / 7.5; // Asume un valor de calibración
                contadorPulsos1 = 0;

                lcd.clear();
                lcd.print("Cliente 1: ");
                lcd.setCursor(0, 1);
                lcd.print("Litros: ");
                lcd.print(litros);

                DynamicJsonDocument response(1024);
                response["bomba"] = 1;
                response["litros"] = litros;
                serializeJson(response, Serial);
            } else if (cliente == 2) {
                digitalWrite(bomba2Pin, HIGH);
                delay(5000); // Simula el tiempo de operación de la bomba
                digitalWrite(bomba2Pin, LOW);

                float litros = contadorPulsos2 / 7.5; // Asume un valor de calibración
                contadorPulsos2 = 0;

                lcd.clear();
                lcd.print("Cliente 2: ");
                lcd.setCursor(0, 1);
                lcd.print("Litros: ");
                lcd.print(litros);

                DynamicJsonDocument response(1024);
                response["bomba"] = 2;
                response["litros"] = litros;
                serializeJson(response, Serial);
            }
        }
    }
}