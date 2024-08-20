import React, { useEffect, useState } from "react";
import ReactApexChart from "react-apexcharts";
import { fetchOrders } from "../../services/DashboardService";

const PieChart = () => {
  const [orders, setOrders] = useState([]); 
  const [chartData, setChartData] = useState({
    series: [],
    labels: [],
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const ordersData = await fetchOrders();
        setOrders(ordersData.data.$values)
        // Calculate the count of orders by status
        const statusCounts = orders.reduce((acc, order) => {
          acc[order.status] = (acc[order.status] || 0) + 1;
          return acc;
        }, {});

        // Prepare data for the chart
        const series = Object.values(statusCounts);
        const labels = Object.keys(statusCounts);

        setChartData({ series, labels });
      } catch (error) {
        console.error("Error fetching orders:", error);
      }
    };

    fetchData();
  }, [orders]);

  const chartOptions = {
    chart: {
      width: 380,
      type: 'pie',
    },
    labels: chartData.labels,
    responsive: [{
      breakpoint: 480,
      options: {
        chart: {
          width: 200
        },
        legend: {
          position: 'bottom'
        }
      }
    }]
  };

  return (
    <div>
      <div id="chart">
        <ReactApexChart options={chartOptions} series={chartData.series} type="pie" width={380} />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default PieChart;
