import * as THREE from 'three';
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';

export default function init({ modelData, imageBase64 }) {
	console.log("modelData geldi:", modelData);

	let regionName = "";
	let blenderPath = "";
	let base64Image = "";

	const bodyCardList = document.querySelectorAll('.bodyCard');

	bodyCardList.forEach((card) => {
		card.addEventListener('click', () => {
			const key = card.querySelector('h5').innerText.trim();
			console.log(`Seçilen Konum: ${key}`);

			regionName = modelData[key][2]; // örnek: "yanakSag"
			blenderPath = modelData[key][1];
			base64Image = imageBase64;

			console.log(`Bölge: ${regionName}\nGLB: ${blenderPath}\nBase64: ${base64Image}`);
			showModel(regionName, blenderPath, base64Image);
		});
	});

	function showModel(regionName, blenderPath, base64Image) {
		if (!regionName || !blenderPath || !base64Image) {
			console.error("Eksik veri!");
			return;
		}
		let controls;

		const container = document.getElementById("3dModelArea");
		container.innerHTML = "";

		const scene = new THREE.Scene();
		scene.background = new THREE.Color(0x1e1e1e);
		const camera = new THREE.PerspectiveCamera(75, container.clientWidth / container.clientHeight, 0.01, 1000);
		const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
		renderer.setSize(container.clientWidth, container.clientHeight);
		renderer.outputEncoding = THREE.sRGBEncoding;
		container.appendChild(renderer.domElement);
		renderer.toneMapping = THREE.ACESFilmicToneMapping;
		renderer.toneMappingExposure = 1.4; // biraz artır, cilt tonu doygun olur



		const hemiLight = new THREE.HemisphereLight(0xffffff, 0x444444, 1.2);
		scene.add(hemiLight);

		const frontLight = new THREE.DirectionalLight(0xffffff, 1.2);
		frontLight.position.set(2, 2, 2);
		scene.add(frontLight);

		const loader = new GLTFLoader();
		loader.load(blenderPath, (gltf) => {
			const model = gltf.scene;
			scene.add(model);

			const box = new THREE.Box3().setFromObject(model);
			const size = box.getSize(new THREE.Vector3());
			const center = box.getCenter(new THREE.Vector3());
			const cameraZ = Math.max(size.x, size.y, size.z) * 1.2; // 🔧 daha yakınlaştırılmış

			camera.position.set(center.x, center.y, cameraZ);
			camera.lookAt(center);

			controls = new OrbitControls(camera, renderer.domElement);
			controls.enableDamping = true;
			controls.dampingFactor = 0.05;
			controls.autoRotate = true;
			controls.autoRotateSpeed = 0.5;
			controls.target.copy(center);

			// Ek ayarlar
			controls.minDistance = size.length() * 0.3; // daha fazla yakınlaşma
			controls.maxDistance = size.length() * 1.5;
			controls.enablePan = false;
			controls.enableZoom = true;
			controls.update();


			// 🔽 skin.jpg yükle
			const skinTexture = new THREE.TextureLoader().load('/img/skin.jpg');
			skinTexture.encoding = THREE.sRGBEncoding;

			const skinMaterial = new THREE.MeshPhysicalMaterial({
				map: skinTexture,
				roughness: 0.8,
				metalness: 0.0,
				clearcoat: 1.0,
				clearcoatRoughness: 0.03,
				reflectivity: 0.5,
				transmission: 0.0,
				ior: 1.4,
				side: THREE.DoubleSide
			});





			// 🔽 hastalık base64 yükle
			const img = new Image();
			img.src = base64Image.startsWith("data:image")
				? base64Image
				: `data:image/png;base64,${base64Image}`;

			img.onload = () => {
				const canvas = document.createElement('canvas');
				canvas.width = img.width;
				canvas.height = img.height;
				const ctx = canvas.getContext('2d');

				// 🔽 önce deri görselini yükle
				const skinImg = new Image();
				skinImg.src = '/img/skin.jpg';

				skinImg.onload = () => {
					// 1. Alta skin.jpg dokusunu çiz
					ctx.drawImage(skinImg, 0, 0, canvas.width, canvas.height);

					// 2. Çarpan blend ile hastalık dokusunu karıştır
					ctx.globalCompositeOperation = 'multiply'; // 🔥 deriyle renk uyumlu hale getirir
					ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

					// 3. Alpha’yı düzgün işleyebilmek için tekrar alpha blend yap
					ctx.globalCompositeOperation = 'destination-in';
					ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

					// 🔽 Texture oluştur
					const blendedTexture = new THREE.Texture(canvas);
					blendedTexture.needsUpdate = true;
					blendedTexture.encoding = THREE.sRGBEncoding;

					const diseaseMaterial = new THREE.MeshStandardMaterial({
						map: blendedTexture,
						transparent: true,
						alphaTest: 0.05,
						depthWrite: false,
						opacity: 1.0,
						side: THREE.DoubleSide
					});

					// 🔽 Model üzerine sadece hastalık bindir
					const materialName = `Mat_${regionName}`;
					model.traverse((child) => {
						if (child.isMesh) {
							if (Array.isArray(child.material)) {
								child.material.forEach((mat) => {
									if (mat.name === materialName) {
										const overlay = child.clone();
										overlay.material = diseaseMaterial;
										scene.add(overlay);
									}
								});
							} else {
								if (child.material.name === materialName) {
									const overlay = child.clone();
									overlay.material = diseaseMaterial;
									scene.add(overlay);
								}
							}
						}
					});
				};
			};

		}, undefined, (error) => {
			console.error("GLB yükleme hatası:", error);
		});

		function animate() {
			requestAnimationFrame(animate);
			if (controls) controls.update();
			renderer.render(scene, camera);
		}
		animate();
	}
}
