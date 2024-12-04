import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "react-toastify/dist/ReactToastify.css";
import { getUserShipmentDetails } from "../../services/shipmentService";
import { useDispatch, useSelector } from "react-redux";
import {
  selectShipment,
  setShipment,
} from "../../redux/slices/shipmentSlice";
import UpdateShipment from "../Payment/UpdateShipment";
import DeleteShipment from "../Payment/DeleteShipment";
import AddShipment from "../Payment/AddShipment";
import { useTranslation } from "react-i18next";

const UserShipment = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const shipment = useSelector(selectShipment);
  const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
  const [currentShipment, setCurrentShipment] = useState(null);
  const { t } = useTranslation();
  const [shipments, setShipments] = useState([])
  const token = localStorage.getItem("token");

  useEffect(() => {
    const getShipment = async () => {
      try {
        if (token) {
          const shipmentData = await getUserShipmentDetails(token);
          dispatch(setShipment(shipmentData.$values));
          setShipments(shipmentData.$values)
          // console.log(shipments);
        }
      } catch (error) {
        console.error("Error fetching shipment:", error);
      }
    };

    getShipment();
  }, [dispatch]);

  useEffect(() => {
  }, [shipments,dispatch]);

  const refreshShipments = async () => {
    try {
      
      if (token) {
        const shipmentData = await getUserShipmentDetails(token);
        dispatch(setShipment(shipmentData.$values));
      }
    } catch (error) {
      console.error("Error refreshing shipments:", error);
    }
  };

  const openUpdateModal = (shipment) => {
    setCurrentShipment(shipment);
    setIsUpdateModalOpen(true);
  };

  const closeUpdateModal = () => {
    setIsUpdateModalOpen(false);
    setCurrentShipment(null);
  };

  return (
    <div className="container mx-auto pt-2 rounded-lg max-w-4xl">
      <div className="flex items-center justify-between mb-6">
        <h2 className="font-bold text-2xl text-orange-500">{t("user_shipment.address")}</h2>
        <AddShipment refreshShipments={refreshShipments} />
      </div>

      {/* No shipments available */}
      {shipments.length === 0 ? (
        <p className="text-gray-500">{t("user_shipment.empty")}</p>
      ) : (
        <div className="space-y-6">
          {/* Shipment List */}
          {shipments.map((shipment) => (
            <div key={shipment.id} className="p-4 bg-gray-100 border rounded-lg flex justify-between items-center">
              <div>
                <div className="flex items-center space-x-2">
                  <span className="font-semibold">{shipment.fullName}</span>
                  <span className="border-l-2 pl-2 text-gray-600">{shipment.phoneNumber}</span>
                </div>
                <p className="text-gray-700">{shipment.address}</p>
              </div>

              {/* Action Buttons */}
              <div className="flex space-x-4">
                <button
                  onClick={() => openUpdateModal(shipment)}
                  className="px-4 py-2 rounded-lg text-orange-500 hover:bg-orange-500 hover:text-white transition-all"
                >
                  {t("user_shipment.update")}
                </button>
                <DeleteShipment id={shipment.id} token={token} />
              </div>
            </div>
          ))}
        </div>
      )}
      {isUpdateModalOpen && (
        <UpdateShipment shipment={currentShipment} onClose={closeUpdateModal} />
      )}
    </div>
  );
};

export default UserShipment;
