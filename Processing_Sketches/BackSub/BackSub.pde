
import processing.video.*;
import blobDetection.*;
import oscP5.*;
import netP5.*;

Capture video;
BlobDetection blobDetect;

OscP5 osc;
NetAddress targetLocation;

PImage baseImg = new PImage();
PImage blobImg = new PImage();

int[] backgroundPixels = new int[480 * 480];

boolean isCalibrated = false;
boolean frameAvailable = false;

String cameraName = "HD Pro Webcam C920";

float currentThreshold = 0.5f;

void setup() {
  size(1440, 480);
  frameRate(5);
  
  setupOsc();
  setupVideo();
  setupBlobDetection();
  
}

void setupOsc() {
  
  osc = new OscP5(this, 6200);
  
  // Target self
  targetLocation = new NetAddress("127.0.0.1", 6201);
  
  // Target other pc
  // targetLocation = new NetAdress("10.3.4.5", 6201);
  
}

void setupVideo() {
  
  for(int i = 0; i < Capture.list().length; i++) {
    if(Capture.list()[i].contains(cameraName)) {
      video = new Capture(this, 480, height, Capture.list()[i], 5);
      break;
    }
  }
  
  video.start();
  
}

void setupBlobDetection() {
  
  blobImg = new PImage(80, 80);
  
  blobDetect = new BlobDetection(blobImg.width, blobImg.height);
  blobDetect.setPosDiscrimination(true);
  blobDetect.setThreshold(currentThreshold);
  
}

void calibrate() {
  println("Calibrated");
  video.loadPixels();
  arrayCopy(video.pixels, backgroundPixels);
  baseImg = new PImage(480, 480, backgroundPixels, false, this);
  isCalibrated = true;
}

void captureEvent(Capture _video) {
  _video.read();
  if(!isCalibrated) {
    calibrate();
  }
  frameAvailable = true;
}

void oscEvent(OscMessage _message) {
  if(_message.addrPattern().contains("/Zaephus/Calibrate")) {
    println("Received a calibrate message!");
    calibrate();
  }
  else {
    print("Received an osc message");
    print(_message.addrPattern());
    println(_message.arguments()[0]);
  }
}

void draw() {
  
  if(!frameAvailable) {
    return;
  }
  
  image(video, 0, 0);
  image(baseImg, 480, 0);
  
  video.loadPixels();
  
  PImage fin = new PImage(480, 480, computeDifference(backgroundPixels, video.pixels), false, this);
  
  fin.filter(BLUR, 2);
  image(fin, 960, 0);
  
  blobImg.copy(fin, 0, 0, fin.width, fin.height, 0, 0, blobImg.width, blobImg.height);
  blobDetect.computeBlobs(blobImg.pixels);
  debugDrawBlobs(960, 0, 480, 480, true, true);
  
  PVector avgPos = computeAvgPos();
  oscSendVector(avgPos);
  
  strokeWeight(3);
  stroke(0, 0, 255);
  line(960 + fin.width / 2, fin.height / 2, 960 + (fin.width / 2) + avgPos.x * 50, (fin.height / 2) + avgPos.y * 50);
  fill(255, 0, 255);
  noStroke();
  
}

void keyPressed() {
  if(key == 'w') {
    currentThreshold += 0.05f;
  }
  else if(key == 's') {
    currentThreshold -= 0.05f;
  }
  blobDetect.setThreshold(currentThreshold);
  
  if(key == 'q') {
    OscMessage message = new OscMessage("/Zaephus/Calibrate");
    message.add(1);
    osc.send(message, targetLocation);
  }
}

PVector computeAvgPos() {
  
  PVector avgPos = new PVector(0, 0);
  
  int blobAmount = blobDetect.getBlobNb();
  
  if(blobAmount <= 0) {
    return avgPos;
  }
  
  for(int i = 0; i < blobAmount; i++) {
    PVector tempVec = new PVector(map(blobDetect.getBlob(i).x, 0, 1, -1, 1), map(blobDetect.getBlob(i).y, 0, 1, -1, 1));
    avgPos.add(tempVec);
  }
  
  avgPos.div(blobAmount);
  
  return avgPos;
  
}

int[] computeDifference(int[] background, int[] compare) {
  
  int[] pix = new int[background.length];
  
  for(int i = 0; i < background.length; i++) {
    
    color compareColor = compare[i];
    color bacgroundColor = background[i];
    
    int compareR = (compareColor >> 16) & 0xFF;
    int compareG = (compareColor >> 8) & 0xFF;
    int compareB = compareColor & 0xFF;
    
    int backgroundR = (bacgroundColor >> 16) & 0xFF;
    int backgroundG = (bacgroundColor >> 8) & 0xFF;
    int backgroundB = bacgroundColor & 0xFF;
    
    int diffR = abs(compareR - backgroundR);
    int diffG = abs(compareG - backgroundG);
    int diffB = abs(compareB - backgroundB);
    
    int threshold = 50;
    
    if(diffR > threshold || diffG > threshold || diffB > threshold) {
      pix[i] = color(255, 255, 255);
    }
    else {
      pix[i] = color(0, 0, 0);
    }
  }
  
  return pix;
  
}

void oscSendVector(PVector _vector) {
  
  OscMessage message = new OscMessage("/Zaephus/Vector");
  
  message.add(_vector.x);
  message.add(_vector.y);
  
  osc.send(message, targetLocation);
  
}

void debugDrawBlobs(float _x, float _y, float _w, float _h, boolean _drawBounds, boolean _drawOutline) {
  
  noFill();
  
  for(int i = 0; i < blobDetect.getBlobNb(); i++) {
    
    Blob blob = blobDetect.getBlob(i);
    
    if(blob != null) {
      
      if(_drawOutline) {
        
        strokeWeight(3);
        stroke(0, 255, 0);
        
        for(int j = 0; j < blob.getEdgeNb(); j++) {
          
          EdgeVertex vertA = blob.getEdgeVertexA(j);
          EdgeVertex vertB = blob.getEdgeVertexB(j);
          
          if(vertA != null && vertB != null) {
            line(_x + vertA.x * _w, _y + vertA.y * _h, _x + vertB.x * _w, _y + vertB.y * _h);
          }
          
        }
        
      }
      
      if(_drawBounds) {
        strokeWeight(1);
        stroke(255, 0, 0);
        rect(_x + blob.xMin * _w, _y + blob.yMin * _h, blob.w * _w, blob.h * _h);
      }
      
    }
    
  }
  
}
