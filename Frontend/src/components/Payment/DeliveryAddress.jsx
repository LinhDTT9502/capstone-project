import React, { useEffect, useState } from "react";
import { Input, Button, Radio } from "@material-tailwind/react";
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
import AddressForm from "../AddressForm";
import { faLocationDot, faVenusMars } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const DeliveryAddress = ({
  userData,
  setUserData,
  setIsEditing,
  setDistance,
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const user = useSelector(selectUser);
  const shipments = useSelector(selectShipment);
  // console.log(shipments);

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
    if (user && shipment) {
      setUserData((prevData) => ({
        ...prevData,
        fullName: shipment?.fullName || prevData.fullName,
        email: shipment?.email || prevData.email,
        address: shipment?.address || prevData.address,
        phoneNumber: shipment?.phoneNumber || prevData.phoneNumber,
        shipmentDetailID: shipment?.id
      }));
    }
  }, [user, shipment, setUserData]);

  // Monitor changes in userData
  // useEffect(() => {
  //   console.log("Updated userData:", userData);
  // }, [userData]);

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

  const handleAddressChange = (fullAddress) => {
    setUserData((prevData) => ({ ...prevData, address: fullAddress }));
  };
  const handleGenderChange = (e) => {
    setUserData((prevData) => ({ ...prevData, gender: e.target.value }));
  };

  return (
    <div className="w-full bg-white border border-gray-200 rounded-lg shadow-md p-3 space-y-2">
      {!user ? (
        <div className="w-full bg-white border border-gray-200 rounded-lg shadow-md p-4 mb-4 space-y-2">
          <h4 className="text-lg font-semibold mb-4">
            <FontAwesomeIcon
              icon={faLocationDot}
              style={{ color: "#ff0000" }}
            />{" "}
            {t("payment.selected_shipment")}:
          </h4>
          <Input
            type="text"
            label={t("payment.full_name")}
            name="fullName"
            value={userData.fullName}
            onChange={(e) =>
              setUserData((prevData) => ({
                ...prevData,
                fullName: e.target.value,
              }))
            }
            required
          />
          <Input
            type="text"
            label="Email"
            name="email"
            value={userData.email}
            onChange={(e) =>
              setUserData((prevData) => ({
                ...prevData,
                email: e.target.value,
              }))
            }
            required
          />
          <Input
            type="text"
            label={t("payment.phone_number")}
            name="phoneNumber"
            value={userData.phoneNumber}
            onChange={(e) =>
              setUserData((prevData) => ({
                ...prevData,
                phoneNumber: e.target.value,
              }))
            }
            required
          />

          {/* <DeliveryAddress userData={userData} setUserData={setUserData} /> */}
          <AddressForm onAddressChange={handleAddressChange} />

          {/* <AddressForm onAddressChange={handleAddressChange} /> */}
        </div>
      ) : shipments.length > 0 ? (
        <>
          {shipment && (
            <div className="w-full bg-white border border-gray-200 rounded-lg shadow-md p-4 mb-4 space-y-2 flex">
              <div className="w-4/5">
                <h4 className="text-lg font-semibold mb-4">
                  <FontAwesomeIcon
                    icon={faLocationDot}
                    style={{ color: "#ff0000" }}
                  />{" "}
                  {t("payment.selected_shipment")}:
                </h4>
                <p className="text-gray-700">
                  <span className="font-semibold">
                    {t("payment.full_name")}:
                  </span>{" "}
                  {shipment.fullName}
                </p>
                <p className="text-gray-700">
                  <span className="font-semibold">Email:</span> {shipment.email}
                </p>
                <p className="text-gray-700">
                  <span className="font-semibold">{t("payment.address")}:</span>{" "}
                  {shipment.address}
                </p>
                <p className="text-gray-700">
                  <span className="font-semibold">
                    {t("payment.phone_number")}:
                  </span>{" "}
                  {shipment.phoneNumber}
                </p>
              </div>
              <div className="flex items-center justify-center w-1/5">
                <ShipmentList />
              </div>
            </div>
          )}
          {!shipment && (
            <div className="">
              <ShipmentList />
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
      )}
      <div className="w-full bg-white border border-gray-200 rounded-lg shadow-md p-3 space-y-2">
        <div className="text-lg font-semibolds">
          <h4>
            <FontAwesomeIcon icon={faVenusMars} style={{ color: "#ff0000" }} />{" "}
            Giới tính:
          </h4>
        </div>
        <div className=" flex gap-10">
          <Radio
            name="gender"
            label="Anh"
            value="Male"
            onChange={handleGenderChange}
            checked={userData.gender === "Male"}
          />
          <Radio
            name="gender"
            label="Chị"
            value="Female"
            onChange={handleGenderChange}
            checked={userData.gender === "Female"}
            className="border-2"
          />
        </div>
      </div>
    </div>
  );
};

export default DeliveryAddress;
