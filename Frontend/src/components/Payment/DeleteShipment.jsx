import React from "react";
import { deleteUserShipmentDetail } from "../../services/shipmentService";
import { useTranslation } from "react-i18next";

export default function DeleteShipment({ id }) {
  const { t } = useTranslation();
  const handleDeleteShipment = async () => {
    try {
      const response = await deleteUserShipmentDetail(id, token);
      // console.log(response);
    } catch (error) {
      console.error("Error deleting shipment:", error);
    }
  };

  return (
    <>
        <button
          type="button"
          onClick={handleDeleteShipment}
          className="rounded-lg p-2 text-orange-500 hover:bg-orange-500 hover:text-white"
        >
          {t("payment.delete")}
        </button>
    </>
  );
}
