"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection
  .start()
  .then(function () {})
  .catch(function (err) {});

document.getElementById("callButton").disabled = true;

const servers = {
  iceServers: [
    {
      urls: ["stun:stun1.l.google.com:19302", "stun:stun2.l.google.com:19302"],
    },
  ],
  iceCandidatePoolSize: 10,
};

// Global State
const pc = new RTCPeerConnection(servers);

console.log("ICE Connection State: ", pc.iceConnectionState);
let localStream = null;
let remoteStream = null;

// HTML elements
const webcamButton = document.getElementById("webcamButton");
const webcamVideo = document.getElementById("webcamVideo");
const callButton = document.getElementById("callButton");
const callInput = document.getElementById("callInput");
const answerButton = document.getElementById("answerButton");
const remoteVideo = document.getElementById("remoteVideo");
const hangupButton = document.getElementById("hangupButton");

// 1. Setup media sources

webcamButton.onclick = async () => {
  localStream = await navigator.mediaDevices.getUserMedia({
    audio: true,
    video: true,
  });
  remoteStream = new MediaStream();

  // Push tracks from local stream to peer connection
  localStream.getTracks().forEach((track) => {
    pc.addTrack(track, localStream);
  });

  // Pull tracks from remote stream, add to video stream
  pc.ontrack = (event) => {
    event.streams[0].getTracks().forEach((track) => {
      remoteStream.addTrack(track);
    });
  };

  webcamVideo.srcObject = localStream;
  remoteVideo.srcObject = remoteStream;

  callButton.disabled = false;
  answerButton.disabled = false;
  webcamButton.disabled = true;
};

callButton.addEventListener("click", function (event) {
  const offerCandidates = [];
  pc.onicecandidate = (event) => {
    if (event.candidate) {
      offerCandidates.push(event.candidate.toJSON());
    } else {
      const offerCandidatesJSON = JSON.stringify(offerCandidates);
      console.log("This is the data in array: " + offerCandidates);
      console.log("The Type of offerCandidates is: " + typeof offerCandidates);
      connection
        .invoke("SetOfferCandidates", offerCandidatesJSON)
        .catch(function (err) {
          return console.error(err.toString());
        });
    }
  };
  // const offerDescription = pc.createOffer();
  // pc.setLocalDescription(offerDescription);
  // console.log("OfferDescription: " + offerDescription.toString());
  // const offer = {
  //   sdp: offerDescription.sdp,
  //   type: offerDescription.type,
  // };
  // console.log("sdp: " + offer.type);
  // const jsonOffer = JSON.stringify(offer);
  // console.log("The Type of offer is: " + typeof jsonOffer);
  // console.log("offer value: " + jsonOffer);

  // connection.invoke("SetOffer", jsonOffer).catch(function (err) {
  //   return console.error(err.toString());
  // });
});

// callButton.addEventListener("click", function (event) {});

callButton.onclick = async () => {
  const offerDescription = await pc.createOffer();
  pc.setLocalDescription(offerDescription);
  const offer = {
    sdp: offerDescription.sdp,
    type: offerDescription.type,
  };
  console.log("sdp: " + offer.sdp);
  const jsonOffer = JSON.stringify(offer);
  console.log("The Type of offer is: " + typeof jsonOffer);
  console.log("offer value: " + jsonOffer);
  connection.invoke("SetOffer", jsonOffer).catch(function (err) {
    return console.error(err.toString());
  });
};

// 3. Answer the call with the unique ID
answerButton.onclick = async () => {
  const callId = callInput.value;
  const callDoc = firestore.collection("calls").doc(callId);
  const answerCandidates = callDoc.collection("answerCandidates");
  const offerCandidates = callDoc.collection("offerCandidates");

  pc.onicecandidate = (event) => {
    event.candidate && answerCandidates.add(event.candidate.toJSON());
  };

  const callData = (await callDoc.get()).data();

  const offerDescription = callData.offer;
  await pc.setRemoteDescription(new RTCSessionDescription(offerDescription));

  const answerDescription = await pc.createAnswer();
  await pc.setLocalDescription(answerDescription);

  const answer = {
    type: answerDescription.type,
    sdp: answerDescription.sdp,
  };

  await callDoc.update({ answer });

  offerCandidates.onSnapshot((snapshot) => {
    snapshot.docChanges().forEach((change) => {
      console.log(change);
      if (change.type === "added") {
        let data = change.doc.data();
        pc.addIceCandidate(new RTCIceCandidate(data));
      }
    });
  });
};
