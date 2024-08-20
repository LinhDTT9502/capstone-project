import React, { useEffect, useState } from "react";
import axios from "axios";
import { Card, CardBody, CardHeader } from "@material-tailwind/react";
import Chart from "react-apexcharts";

const BarChart = () => {
  const [chartData, setChartData] = useState({
    series: [
      {
        name: "Tổng số đơn đặt hàng",
        data: [],
      },
      {
        name: "Tổng doanh thu",
        data: [],
      },
    ],
    categories: [
      "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    ],
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const months = Array.from({ length: 12 }, (_, i) => i + 1);
        const totalOrders = [];
        const totalIntoMoney = [];

        for (const month of months) {
          const response = await axios.get(`https://twosportapiv2.azurewebsites.net/api/Order/get-orders-sales?month=${month}`);
          if (response.data.isSuccess) {
            totalOrders.push(response.data.data.totalOrders);
            totalIntoMoney.push(response.data.data.totalIntoMoney);
          } else {
            totalOrders.push(0);
            totalIntoMoney.push(0);
          }
        }

        setChartData({
          series: [
            { name: "Tổng số đơn đặt hàng", data: totalOrders },
            { name: "Tổng doanh thu", data: totalIntoMoney },
          ],
          categories: [
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
          ],
        });
      } catch (error) {
        console.error("Error fetching data: ", error);
      }
    };

    fetchData();
  }, []);

  const chartConfig = {
    type: "bar",
    height: 240,
    series: chartData.series,
    options: {
      chart: {
        toolbar: { show: false },
      },
      title: { show: "" },
      dataLabels: { enabled: false },
      colors: ["#020617", "#4CAF50"], // Add colors for both series
      plotOptions: {
        bar: { columnWidth: "40%", borderRadius: 2 },
      },
      xaxis: {
        axisTicks: { show: false },
        axisBorder: { show: false },
        labels: {
          style: { colors: "#616161", fontSize: "12px", fontFamily: "inherit", fontWeight: 400 },
        },
        categories: chartData.categories,
      },
      yaxis: {
        labels: {
          style: { colors: "#616161", fontSize: "12px", fontFamily: "inherit", fontWeight: 400 },
        },
      },
      grid: {
        show: true,
        borderColor: "#dddddd",
        strokeDashArray: 5,
        xaxis: { lines: { show: true } },
        padding: { top: 5, right: 20 },
      },
      fill: { opacity: 0.8 },
      tooltip: { theme: "dark" },
    },
  };

  return (
    <Card>
      <CardHeader
        floated={false}
        shadow={false}
        color="transparent"
        className="flex flex-col gap-4 rounded-none md:flex-row md:items-center"
      >
        Doanh thu theo tháng
      </CardHeader>
      <CardBody className="px-2 pb-0">
        <Chart {...chartConfig} />
      </CardBody>
    </Card>
  );
};

export default BarChart;
