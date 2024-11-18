import { initializeApp } from "firebase/app";
import { getFirestore, doc, setDoc, getDoc } from "firebase/firestore";
import { getAuth } from "firebase/auth";
const apiKey = import.meta.env.VITE_API_KEY;

const firebaseConfig = {
  apiKey: apiKey,
  authDomain: "sport-d1e33.firebaseapp.com",
  projectId: "sport-d1e33",
  storageBucket: "sport-d1e33.firebasestorage.app",
  messagingSenderId: "329977845646",
  appId: "1:329977845646:web:d8ff6cad563e1ac58436df"
};

const app = initializeApp(firebaseConfig);
export const db = getFirestore(app);
export const auth = getAuth(app);
