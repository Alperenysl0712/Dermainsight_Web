import init from './3DModelDetail.js';

// Window üzerinden gelen Razor verilerini al
const modelData = window.modelData;
const imageBase64 = window.imageBase64;

init({ modelData, imageBase64 });
