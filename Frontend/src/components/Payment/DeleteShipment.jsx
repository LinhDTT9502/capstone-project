import React from "react";
import { deleteUserShipmentDetail } from "../../services/shipmentService";
import { useTranslation } from "react-i18next";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

export default function DeleteShipment({ id, token, setReload }) {
  const { t } = useTranslation();
  const handleDeleteShipment = async () => {
    const confirmDelete = window.confirm(
      "Bạn có chắc chắn muốn xóa địa chỉ này?"
    );
    if (!confirmDelete) return;

    try {
      await deleteUserShipmentDetail(id, token);
      // console.log(response);
      toast.success("Xóa địa chỉ thành công!");
      setReload();
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
