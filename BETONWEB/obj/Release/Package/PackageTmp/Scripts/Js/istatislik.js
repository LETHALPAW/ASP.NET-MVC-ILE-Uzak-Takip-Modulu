function sortData(data, valueType, labelType) {
    const total = data.reduce((sum, item) => sum + item[valueType], 0);
    return data.map(item => ({
        value: item[valueType],
        name: item[labelType],
        percentage: ((item[valueType] / total) * 100).toFixed(2)
    })).sort((a, b) => b.value - a.value);
}

function createPieChart(elementId, data, seriesName, valueType, labelType) {
    // Veri toplamını hesapla

    const sortedData = sortData(data, valueType, labelType);


    // Grafik seçeneklerini ayarla
    var chartOptions = {
        tooltip: {
            trigger: 'item',
            formatter: '{a} <br/>{b} : {c} ({d}%)'
        },
        legend: {
            orient: 'vertical',
            left: 'left',
            type: 'scroll',
            formatter: function (name) {
                var item = sortedData.find(item => item.name === name);
                return `${name} (${item.percentage}%)`;
            },
            data: sortedData.map(item => item.name)
        },
        series: [
            {
                name: seriesName,
                type: 'pie',
                radius: [20, 140],
                center: ['70%', '50%'],
                data: sortedData,
                roseType: 'area',
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                },
                 itemStyle: {
                    borderRadius: 5
                }
            }
        ],
        responsive: true
    };

    // ECharts elementini başlat
    var chartElement = document.getElementById(elementId);
    if (chartElement) {
        var chart = echarts.init(chartElement);
        chart.setOption(chartOptions);
    } else {
        console.error('Element not found for Pie Chart:', elementId);
    }
}


function createLineChart(elementId, data, seriesName, valueType, labelType) {
    // Ay ve ToplamMiktar verilerini dizi olarak hazırlama
    const months = data.map(item => item.Ay);
    const totals = data.map(item => item.ToplamMiktar);

    // ECharts ile çizgi grafiği oluşturma seçenekleri
    const option = {
        xAxis: {
            type: 'category',
            data: months,
            name: labelType
        },
        yAxis: {
            type: 'value',
            name: valueType
        },
        series: [
            {
                name: seriesName,
                data: totals,
                type: 'line'
            }
        ],
        tooltip: {
            trigger: 'axis'
        },
        toolbox: {
            show: true,
            feature: {
                saveAsImage: { show: true, title: 'Save' }
            }
        }
    };

    // ECharts örneğini başlatma ve seçenekleri uygulama
    var chart = echarts.init(document.getElementById(elementId));
    chart.setOption(option);
}


function musteriBazindaSiparisler(ilkTarih, sonTarih) {
    $.ajax({
        url: '/Istatistic/MusteriBazindaSiparislerGetir',
        type: 'GET',
        dataType: 'json',
        data: {
            ilkTarih: ilkTarih,
            sonTarih: sonTarih
        },
        success: function (response) {
            console.log('Data successfully received:', response.data);  // Ensure this logs an array
            if (Array.isArray(response.data)) {
                createBarchart('musteriBazindaSiparisler', response.data);
            } else {
                console.error('Data is not an array:', response.data);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching data:', error);
        }
    });
}


function createBarchart(elementId, data) {
    // Müşteri adlarını ve sipariş değerlerini ayrı listeler olarak hazırla
    var categories = [];
    var siparisIstenen = [];
    var siparisVerilen = [];

    // Verileri döngü ile işle
    data.forEach(function (item) {
        // Müşteri adını kategori listesine ekle
        categories.push(item.MusteriAdi);
        // Sipariş istenen ve verilen değerleri ilgili listelere ekle
        siparisIstenen.push(item.SiparisIstenen);
        siparisVerilen.push(item.SiparisVerilen);
    });

    // ECharts'ı kullanarak grafiği oluştur
    var myChart = echarts.init(document.getElementById(elementId));

    var option = {
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        legend: {
            data: ['Sipariş İstenen', 'Sipariş Verilen']
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: {
            type: 'value',
            boundaryGap: [0, 0.01]
        },
        yAxis: {
            type: 'category',
            data: categories  // Müşteri adları
        },
        series: [
            {
                name: 'Sipariş İstenen',
                type: 'bar',
                data: siparisIstenen  // Sipariş istenen miktarlar
            },
            {
                name: 'Sipariş Verilen',
                type: 'bar',
                data: siparisVerilen  // Sipariş verilen miktarlar
            }
        ]
    };

    // Grafiği belirlenen seçeneklerle oluştur
    myChart.setOption(option);
}
