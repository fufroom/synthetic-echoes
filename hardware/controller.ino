#include <Wire.h>

#define CAP1188_I2C_ADDRESS 0x29 
#define REG_TOUCHED_STATUS 0x03 
#define REG_MAIN_CONTROL 0x00    
#define REG_SENSITIVITY 0x1F     
#define REG_C8_THRESHOLD 0x37    
#define REG_CALIBRATE 0x26       

#define RED_PIN 9
#define GREEN_PIN 10
#define BLUE_PIN 11

bool isTouched = false;
unsigned long lastRecalibration = 0;

void setup() {
  Serial.begin(9600);
  Wire.begin();

  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);

  // Ensure LEDs are off initially
  turnOffLED();

  Serial.println("Initializing CAP1188...");

  // Verify CAP1188 presence
  if (!devicePresent()) {
    Serial.println("CAP1188 not detected. Check wiring!");
    while (true) {
      delay(1000); // Halt if device is not detected
    }
  }
  Serial.println("CAP1188 detected!");

  // Apply saved sensitivity settings
  writeRegister(REG_SENSITIVITY, 0x10);  // Medium sensitivity
  Serial.println("Sensitivity set to medium.");

  // Increase threshold for C8 to avoid false positives
  writeRegister(REG_C8_THRESHOLD, 0x40); 
  Serial.println("C8 threshold increased.");

  // Initial calibration to clear noise
  recalibrateC8();
}

void loop() {
  uint8_t touchStatus = readRegister(REG_TOUCHED_STATUS);

  // Check only C8 (bit 7)
  if (touchStatus & (1 << 7)) {
    if (!isTouched) { 
      Serial.println("Touch detected on C8!");
      setAmberColor();
      isTouched = true;
    }
  } else {
    if (isTouched) {
      Serial.println("Touch ended on C8!");
      turnOffLED();
      isTouched = false;
    }
  }

  // Periodic recalibration to handle environmental changes
  if (millis() - lastRecalibration > 5000) {
    recalibrateC8();
    lastRecalibration = millis();
  }

  // Clear touch state in the CAP1188 to ensure proper updates
  clearTouchState();

  delay(200); 
}

void setAmberColor() {
  analogWrite(RED_PIN, 255);   // Full red brightness
  analogWrite(GREEN_PIN, 128); // Medium green brightness
  analogWrite(BLUE_PIN, 0);    // Blue off (amber color)
}

void turnOffLED() {
  analogWrite(RED_PIN, 0);
  analogWrite(GREEN_PIN, 0);
  analogWrite(BLUE_PIN, 0);
}

void recalibrateC8() {
  writeRegister(REG_CALIBRATE, 0xFF);
  Serial.println("Recalibration triggered.");
}

void clearTouchState() {
  writeRegister(REG_MAIN_CONTROL, 0x00);
}

bool devicePresent() {
  uint8_t productID = readRegister(0xFD);
  return (productID == 0x50); 
}

bool writeRegister(uint8_t reg, uint8_t value) {
  Wire.beginTransmission(CAP1188_I2C_ADDRESS);
  Wire.write(reg);
  Wire.write(value);
  return (Wire.endTransmission() == 0);
}

uint8_t readRegister(uint8_t reg) {
  Wire.beginTransmission(CAP1188_I2C_ADDRESS);
  Wire.write(reg);
  if (Wire.endTransmission() != 0) {
    return 0xFF; 
  }
  Wire.requestFrom(CAP1188_I2C_ADDRESS, 1);
  return Wire.available() ? Wire.read() : 0xFF;
}
