const int speakerPin = 9;

void setup() {
  Serial.begin(9600);
  pinMode(speakerPin, OUTPUT);
}

void loop() {
  if (Serial.available()) {
    String command = Serial.readStringUntil('\n');

    if (command.startsWith("LERP")) {
      int startFreq, endFreq, duration;
      sscanf(command.c_str(), "LERP %d %d %d", &startFreq, &endFreq, &duration);
      lerpTone(startFreq, endFreq, duration);
    } 
    else if (command.startsWith("SEQ")) {
      int values[5], duration;
      sscanf(command.c_str(), "SEQ %d,%d,%d,%d,%d %d", &values[0], &values[1], &values[2], &values[3], &values[4], &duration);
      playSequence(values, 5, duration);
    } 
    else if (command.startsWith("GLITCH")) {
      int minFreq, maxFreq, duration, interval;
      sscanf(command.c_str(), "GLITCH %d %d %d %d", &minFreq, &maxFreq, &duration, &interval);
      glitchTone(minFreq, maxFreq, duration, interval);
    } 
    else {
      int freq = command.toInt();
      if (freq > 0) {
        tone(speakerPin, freq);
      } else {
        noTone(speakerPin);
      }
    }
  }
}

void lerpTone(int startFreq, int endFreq, int duration) {
  int step = (endFreq > startFreq) ? 1 : -1;
  int delayTime = duration / abs(endFreq - startFreq);
  for (int freq = startFreq; freq != endFreq; freq += step) {
    tone(speakerPin, freq);
    delay(delayTime);
  }
  noTone(speakerPin);
}

void playSequence(int *frequencies, int length, int duration) {
  int interval = duration / length;
  for (int i = 0; i < length; i++) {
    tone(speakerPin, frequencies[i]);
    delay(interval);
  }
  noTone(speakerPin);
}

void glitchTone(int minFreq, int maxFreq, int duration, int interval) {
  unsigned long startTime = millis();
  while (millis() - startTime < duration) {
    int randFreq = random(minFreq, maxFreq);
    tone(speakerPin, randFreq);
    delay(interval);
    noTone(speakerPin);
    delay(interval / 2);
  }
}
