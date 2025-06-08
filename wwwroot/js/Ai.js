var diseaseNameAi = "";

document.getElementById("imageInput").addEventListener("change", async function (event) {
    const file = event.target.files[0];
    if (!file) {
        alert("Lütfen bir dosya seçin.");
        return;
    }

    if (!file.type.startsWith('image/')) {
        alert("Lütfen sadece resim dosyası seçin (.jpg, .png)");
        return;
    }

    const formData = new FormData();
    formData.append('image', file);

    const reader = new FileReader();
    reader.onload = function (e) {
        const imgElement = document.getElementById("selectedImage");
        imgElement.src = e.target.result;
    };
    reader.readAsDataURL(file);

    try {
        const response = await fetch('/upload-endpoint', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) throw new Error("Yükleme hatası");

        const result = await response.json();
        console.log("Sunucudan gelen:", result);

        // İstersen sonucu ekrana yaz:
        // document.getElementById("sonuc").innerText = JSON.stringify(result);

        if (Array.isArray(result) && result.length > 0) {
            const best = result.reduce((prev, current) =>
                current.probability > prev.probability ? current : prev
            );

            diseaseNameAi = best.class;
            // Sonucu yazdır
            document.getElementById("resultLabel").innerText = `Sonuç: ${diseaseList[best.class]}`;


            const labels = result.map(r => diseaseList[r.class]);
            const data = result.map(r => (r.probability * 100).toFixed(2));

            if (window.resultChart instanceof Chart) {
                window.resultChart.destroy();
            }

            const ctx = document.getElementById('resultChart').getContext('2d');
            window.resultChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Tahmin Dağılımı (%)',
                        data: data,
                        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#8BC34A', '#E91E63', '#03A9F4', '#FF9800'],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    return `${context.label}: ${context.parsed}%`;
                                }
                            }
                        }
                    }
                }
            });

            document.querySelector(".resultSec").style.display = "flex";
            document.querySelector(".validationSec").style.display = "flex";

        } else {
            alert("Tahmin alınamadı.");
        }




    } catch (error) {
        console.error("Hata:", error);
        alert("Yükleme başarısız.");
    }
});

document.getElementById("saveOtherDisease").addEventListener("click", async function () {
    const selected = document.querySelector('input[name="diseaseOption"]:checked');
    if (!selected) {
        alert("Hastalık tipi seçiniz")
        return;
    }
    var selectedValue = selected.value.trim();
    console.log("Seçilen hastalık:", selectedValue);

    const otherDetail = document.getElementById("otherDiseaseInput").value.trim();

    console.log("Seçilen hastalık detayı:", otherDetail);

    if (selectedValue === "other" && otherDetail == "") {
        alert("Lütfen hastalık detayını giriniz. ");
        return;
    }

    try {
        const response = await fetch('/saveDisease', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                diseaseType: selectedValue,
                diseaseDetail: otherDetail
            })
        });

        if (!response.ok) throw new Error("Yükleme hatası");

        const result = await response.json();
        console.log("Sunucudan gelen:", result);

        if (result == true) {
            alert("Başarıyla kaydedildi.");
            const modalEl = document.getElementById('otherDiseaseModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            modalInstance.hide();
            document.querySelector(".validationSec").style.display = "none";
        }

    } catch (error) {
        console.error("Hata:", error);
        alert("Yükleme başarısız.");
    }

});

document.getElementById("saveDiseaseBtn").addEventListener("click", async function () {
    var selectedValue = diseaseNameAi.trim();
    console.log("Seçilen hastalık:", selectedValue);

    const otherDetail = "";

    console.log("Seçilen hastalık detayı:", otherDetail);

    if (selectedValue === "other" && otherDetail == "") {
        alert("Lütfen hastalık detayını giriniz. ");
        return;
    }

    try {
        const response = await fetch('/saveDisease', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                diseaseType: selectedValue,
                diseaseDetail: otherDetail
            })
        });

        if (!response.ok) throw new Error("Yükleme hatası");

        const result = await response.json();
        console.log("Sunucudan gelen:", result);

        if (result == true) {
            alert("Başarıyla kaydedildi.");
            document.querySelector(".validationSec").style.display = "none";

        }

    } catch (error) {
        console.error("Hata:", error);
        alert("Yükleme başarısız.");
    }

});
