import React, { useEffect, useState } from "react";
import { Input, Button } from "@material-tailwind/react";
import { useDispatch, useSelector } from "react-redux";
import SignInModal from "../Auth/SignInModal";
import ShipmentList from "./ShipmentList";
import {
  selectedShipment,
  selectShipment,
  setShipment,
} from "../../redux/slices/shipmentSlice";
import { selectUser } from "../../redux/slices/authSlice";
import DistanceCalculator from "./DistanceCalculator";
import AddShipment from "./AddShipment";
import { useNavigate } from "react-router-dom";
import { addUserShipmentDetail, getUserShipmentDetails } from "../../services/shipmentService";
import { useTranslation } from "react-i18next";

const DeliveryAddress = ({
  userData,
  setUserData,
  setIsEditing,
  setDistance,
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const user = useSelector(selectUser);
  const shipments = useSelector(selectShipment);
  const shipment = useSelector(selectedShipment);
  const token = localStorage.getItem("token");
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { t } = useTranslation();

  useEffect(() => {
    const fetchShipments = async () => {
      try {
        if (token && shipments.length === 0) {
          const shipmentData = await getUserShipmentDetails(token);
          dispatch(setShipment(shipmentData.$values));
        }
      } catch (error) {
        console.error("Error fetching shipment:", error);
      }
    };

    fetchShipments();
  }, [token, dispatch, shipments.length]);

  useEffect(() => {
    if (shipment) {
      setUserData({
        fullName: shipment.fullName,
        address: shipment.address,
        phoneNumber: shipment.phoneNumber,
      });
    }
  }, [shipment, setUserData]);

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    setUserData({ ...userData, [name]: value });
  };

  const handleSaveClick = async (data) => {
    setIsSubmitting(true);
    try {
      const response = await addUserShipmentDetail(token, data);

      if (response.status === 200) {
        setIsSubmitting(false);
        setIsEditing(false);
        alert("Shipment details saved successfully.");
      }
    } catch (error) {
      setIsSubmitting(false);
      console.error("Error saving shipment details:", error);
    }
  };

  const handleCancel = () => {
    navigate("/cart");
  };

  return (
    <div className="pl-20 py-10">
      <div className="flex justify-between items-center">
        <h3 className="text-2xl font-bold">{t("payment.delivery_address")}</h3>
      </div>

      {!user ? (
        <div className="flex items-center">
          <p className="text-xl pr-5">{t("payment.already_have_account")}</p>
          <SignInModal className="text-blue-500" />
        </div>
      ) : (
        shipments.length > 0 ? (
          <>
            <ShipmentList />
            {shipment && (
              <div className="w-fit bg-white border border-gray-200 rounded-lg shadow-md p-6 my-4 space-y-2 ">
                <h4 className="text-lg font-semibold mb-4">{t("payment.selected_shipment")}:</h4>
                <p className="text-gray-700">
                  <span className="font-semibold">{t("payment.full_name")}:</span>{" "}
                  {shipment.fullName}
                </p>
                <p className="text-gray-70">
                  <span className="font-semibold">{t("payment.address")}:</span>{" "}
                  {shipment.address}
                </p>
                <p className="text-gray-700">
                  <span className="font-semibold">{t("payment.phone_number")}:</span>{" "}
                  {shipment.phoneNumber}
                </p>
              </div>
            )}
          </>
        ) : (
          <AddShipment
            onSubmit={handleSaveClick}
            onCancel={handleCancel}
            initialData={userData}
            setUserData={setUserData}
          />
        )
      )}

      {userData.address && (
        <DistanceCalculator
          userAddress={userData.address}
          onDistanceCalculated={(distance) => {
            console.log("Calculated Distance:", distance);
            setDistance(distance);
          }}
        />
      )}
    </div>
  );
};

export default DeliveryAddress;
