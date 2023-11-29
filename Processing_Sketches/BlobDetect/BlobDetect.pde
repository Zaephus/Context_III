
import processing.video.*;
import blobDetection.*;
import oscP5.*;
import netP5.*;

Capture video;
BlobDetection blobDetect;

OscP5 osc;
NetAddress targetLocation;

boolean frameAvailable = false;

PImage blobImage;

PVector direction;

float currentThreshold = 0.5f;

void setup() {
  
  size(640, 640);
  
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
  
  // Logitech Cam
  video = new Capture(this, width, height, Capture.list()[1], 5);
  
  // Built in Cam
  //video = new Capture(this, width, height, Capture.list()[0]);
  
  video.start();
  
}

void setupBlobDetection() {
  
  blobImage = new PImage(80, 80);
  
  blobDetect = new BlobDetection(blobImage.width, blobImage.height);
  blobDetect.setPosDiscrimination(true);
  blobDetect.setThreshold(currentThreshold);
  
}

void oscEvent(OscMessage _message) {
  print("Received an osc message");
  println(_message.arguments()[0]);
}

void captureEvent(Capture _video) {
  _video.read();
  frameAvailable = true;
}

void draw() {
  
  if(frameAvailable) {
    
    video.filter(THRESHOLD, 0.3);
    image(video, 0, 0);
    
    blobImage.copy(video, 0, 0, video.width, video.height, 0, 0, blobImage.width, blobImage.height);
    blobDetect.computeBlobs(blobImage.pixels);
    debugDrawBlobs(true, true);
    
    PVector avgPos = new PVector();
    for(int i = 0; i < blobDetect.getBlobNb(); i++) {
      PVector tempVec = new PVector(map(blobDetect.getBlob(i).x, 0, 1, -1, 1), map(blobDetect.getBlob(i).y, 0, 1, -1, 1));
      avgPos.add(tempVec);
    }
    
    direction = PVector.div(avgPos, blobDetect.getBlobNb());
    oscSendVector(direction);
    
    strokeWeight(3);
    stroke(0, 0, 255);
    line(width / 2, height / 2, (width / 2) + direction.x * 50, (height / 2) + direction.y * 50);
    fill(255, 0, 255);
    noStroke();
    
  }
  
}

void keyPressed() {
  if(key == 'w') {
    currentThreshold += 0.05f;
  }
  else if(key == 's') {
    currentThreshold -= 0.05f;
  }
  blobDetect.setThreshold(currentThreshold);
}

void oscSendVector(PVector _vector) {
  
  OscMessage message = new OscMessage("/Zaephus/Vector");
  
  message.add(_vector.x);
  message.add(_vector.y);
  
  osc.send(message, targetLocation);
  
}

void debugDrawBlobs(boolean _drawBounds, boolean _drawOutline) {
  
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
            line(vertA.x * width, vertA.y * height, vertB.x * width, vertB.y * height);
          }
          
        }
        
      }
      
      if(_drawBounds) {
        strokeWeight(1);
        stroke(255, 0, 0);
        rect(blob.xMin * width, blob.yMin * height, blob.w * width, blob.h * height);
      }
      
    }
    
  }
  
}
