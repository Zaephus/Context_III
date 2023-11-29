import gab.opencv.*;
import processing.video.*;

//Movie video;
Capture video;
OpenCV opencv;

boolean isCalibrated = false;

void setup() {
  size(720, 480);
  video = new Capture(this, width, height, Capture.list()[1], 5);
  opencv = new OpenCV(this, 720, 480);

  opencv.startBackgroundSubtraction(5, 3, 0.5);
  
  video.start();
  //video.loop();
  //video.play();
  
  //calibrate();
}

void calibrate() {
  video.read();
  opencv.loadImage(video);
  opencv.updateBackground();
  isCalibrated = true;
}

void draw() {
  image(video, 0, 0);

  if (video.width == 0 || video.height == 0)
    return;

  //opencv.loadImage(video);
  opencv.updateBackground();

  opencv.dilate();
  opencv.erode();

  noFill();
  stroke(255, 0, 0);
  strokeWeight(3);
  for (Contour contour : opencv.findContours()) {
    contour.draw();
  }
}

void captureEvent(Capture _video) {
  _video.read();
  if(!isCalibrated) {
    calibrate();
  }
}
