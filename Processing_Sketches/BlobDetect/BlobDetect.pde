
import processing.video.*;
import blobDetection.*;

Capture video;
BlobDetection blobDetect;

boolean frameAvailable = false;

PImage blobImage;

PVector direction;

void setup() {
  
  size(640, 640);
  
  video = new Capture(this, width, height, Capture.list()[1], 5);
  video.start();
  
  blobImage = new PImage(80, 80);
  
  blobDetect = new BlobDetection(blobImage.width, blobImage.height);
  blobDetect.setPosDiscrimination(true);
  blobDetect.setThreshold(0.25f);
  
}

void captureEvent(Capture _video) {
  _video.read();
  frameAvailable = true;
}

void draw() {
  
  if(frameAvailable) {
    
    //video.filter(THRESHOLD, 0.3);
    image(video, 0, 0);
    
    blobImage.copy(video, 0, 0, video.width, video.height, 0, 0, blobImage.width, blobImage.height);
    blobDetect.computeBlobs(blobImage.pixels);
    debugDrawBlobs(true, true);
    
    PVector avgPos = new PVector();
    for(int i = 0; i < blobDetect.getBlobNb(); i++) {
      PVector tempVec = new PVector(map(blobDetect.getBlob(i).x, 0, 1, -1, 1), map(blobDetect.getBlob(i).y, 0, 1, -1, 1));
      avgPos.add(tempVec);
      println(tempVec);
    }
    
    direction = PVector.div(avgPos, blobDetect.getBlobNb());
    //println(direction);
    
    strokeWeight(3);
    stroke(0, 0, 255);
    line(width / 2, height / 2, (width / 2) + direction.x * 50, (height / 2) + direction.y * 50);
    fill(255, 0, 255);
    noStroke();
    circle((width / 2) + direction.x * 50, (height / 2) + direction.y * 50, 8);
    
  }
  
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
