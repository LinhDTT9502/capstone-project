import React, { useState, useEffect, Fragment } from "react";
import { Dialog, Transition } from "@headlessui/react";
import { updateUserShipmentDetail } from "../../services/shipmentService";
import { useDispatch } from "react-redux";
import { updateShipment } from "../../redux/slices/shipmentSlice";
import AddressForm from "../AddressForm";
import { useTranslation } from "react-i18next";

export default function UpdateShipment({ shipment, onClose }) {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState(true);
  const [formData, setFormData] = useState({ ...shipment });
  const [address, setAddress] = useState("");
  const token = localStorage.getItem("token");
  const dispatch = useDispatch();

  useEffect(() => {
    setFormData({ ...shipment });
  }, [shipment]);

  const handleUpdateShipment = async () => {
    try {
      const updatedShipment = await updateUserShipmentDetail(
        shipment.id,
        token,
        { ...formData, address }
      );
      dispatch(updateShipment(updatedShipment));
      setIsOpen(false);
      onClose();
    } catch (error) {
      console.error("Error updating shipment details:", error);
    }
  };

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleAddressChange = (newAddress) => {
    setAddress(newAddress);
  };

  function closeModal() {
    setIsOpen(false);
    onClose();
  }

  return (
    <>
      <Transition appear show={isOpen} as={Fragment}>
        <Dialog as="div" className="relative z-10" onClose={closeModal}>
          <Transition.Child
            as={Fragment}
            enter="ease-out duration-300"
            enterFrom="opacity-0"
            enterTo="opacity-100"
            leave="ease-in duration-200"
            leaveFrom="opacity-100"
            leaveTo="opacity-0"
          >
            <Dialog.Overlay className="fixed inset-0 bg-black opacity-50" />
          </Transition.Child>

          <div className="fixed inset-0 overflow-y-auto">
            <div className="flex items-center justify-center min-h-screen">
              <Transition.Child
                as={Fragment}
                enter="ease-out duration-300"
                enterFrom="opacity-0 scale-95"
                enterTo="opacity-100 scale-100"
                leave="ease-in duration-200"
                leaveFrom="opacity-100 scale-100"
                leaveTo="opacity-0 scale-95"
              >
                <Dialog.Panel className="bg-white p-6 rounded-md shadow-xl max-w-md w-full mx-4">
                  <Dialog.Title className="text-xl font-bold">
                    {t("payment.update_shipment")}
                  </Dialog.Title>
                  <div className="mt-4">
                    <label className="block text-sm font-medium text-gray-700">
                      {t("payment.full_name")}
                    </label>
                    <input
                      type="text"
                      name="fullName"
                      value={formData.fullName}
                      onChange={handleInputChange}
                      className="mt-1 block w-full border rounded-md p-2 focus:ring-blue-500 focus:border-blue-500"
                    />
                  </div>
                  <div className="mt-4">
                    <label className="block text-sm font-medium text-gray-700">
                      {t("payment.phone_number")}
                    </label>
                    <input
                      type="tel"
                      name="phoneNumber"
                      value={formData.phoneNumber}
                      onChange={handleInputChange}
                      className="mt-1 block w-full border rounded-md p-2 focus:ring-blue-500 focus:border-blue-500"
                    />
                  </div>
                  <div className="mt-4">
                    <label className="block text-sm font-medium text-gray-700">
                      {t("payment.address")}
                    </label>
                    <AddressForm onAddressChange={handleAddressChange} />
                  </div>
                  <div className="mt-6 flex justify-end">
                    <button
                      type="button"
                      className="bg-gray-500 text-white px-4 py-2 rounded-md mr-2 hover:bg-gray-600"
                      onClick={closeModal}
                    >
                      {t("payment.cancel")}
                    </button>
                    <button
                      type="button"
                      className="bg-orange-500 text-white px-4 py-2 rounded-md hover:bg-orange-700"
                      onClick={handleUpdateShipment}
                    >
                      {t("payment.confirm")}
                    </button>
                  </div>
                </Dialog.Panel>
              </Transition.Child>
            </div>
          </div>
        </Dialog>
      </Transition>
    </>
  );
}
