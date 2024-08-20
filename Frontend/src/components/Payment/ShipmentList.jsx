import { useState,useEffect, Fragment } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  selectShipment,
  selectShipments,
  setShipment,
} from "../../redux/slices/shipmentSlice";
import { Dialog, Transition } from "@headlessui/react";
import "react-toastify/dist/ReactToastify.css";
import UpdateShipment from "./UpdateShipment";
import AddShipment from "./AddShipment";
import { addUserShipmentDetail, getUserShipmentDetails } from "../../services/shipmentService";
import { useTranslation } from "react-i18next";

export default function ShipmentList() {
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const [selectedShipment, setSelectedShipment] = useState(null);
  const [isShipmentListOpen, setIsShipmentListOpen] = useState(true);
  const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
  const [currentShipment, setCurrentShipment] = useState(null);
  // const [shipments, setShipments] = useState([])
  const shipments = useSelector(selectShipment);

  useEffect(() => {
    const getShipment = async () => {
      try {
        const token = localStorage.getItem("token");
        if (token) {
          const shipmentData = await getUserShipmentDetails(token);
          dispatch(setShipment(shipmentData.$values));
          // setShipments(shipmentData.$values)
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
      const token = localStorage.getItem("token");
      if (token) {
        const shipmentData = await getUserShipmentDetails(token);
        dispatch(setShipment(shipmentData.$values));
      }
    } catch (error) {
      console.error("Error refreshing shipments:", error);
    }
  };

  const handleSelectShipment = (shipment) => {
    dispatch(selectShipments(shipment));
    setSelectedShipment(shipment);
    setIsShipmentListOpen(false);
  };

  const openUpdateModal = (shipment) => {
    setCurrentShipment(shipment);
    setIsUpdateModalOpen(true);
    setIsShipmentListOpen(false);
  };

  const closeUpdateModal = () => {
    setIsUpdateModalOpen(false);
    setIsShipmentListOpen(true);
  };

  function closeModal() {
    setIsShipmentListOpen(false);
  }

  function openModal() {
    setIsShipmentListOpen(true);
  }

  return (
    <>
    <button
            type="button"
            onClick={openModal}
            className="border-r-2 pr-4 text-orange-500"
          >
           Địa chỉ đã lưu trữ
          </button>
      {isShipmentListOpen && (
        <div className="mb-4">
          
          <Transition appear show={isShipmentListOpen} as={Fragment}>
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
                    <Dialog.Panel className="bg-white p-6 rounded-md shadow-xl w-fit mx-4">
                      {shipments.length === 0 ? (
                        <p className="text-center text-gray-700">
                          {t("payment.address_book_empty")}
                        </p>
                      ) : (
                        <div>
                          <div className="mb-4">
                            <h2 className="font-bold text-xl text-gray-900">
                              {t("payment.my_address")}
                            </h2>
                          </div>
                          {shipments.map((shipment) => (
                            <div
                              className="p-4 border-b last:border-b-0 flex justify-between items-center"
                              key={shipment.id}
                            >
                              <input
                                type="radio"
                                name="selectedShipment"
                                onChange={() => handleSelectShipment(shipment)}
                                checked={selectedShipment?.id === shipment.id}
                                className=""
                              />
                              <div className=" w-3/5">
                                <div className="flex items-center">
                                  <label className="pr-2 font-medium text-gray-800">
                                    {shipment.fullName}
                                  </label>
                                  <p className="border-l-2 pl-2 text-gray-600">
                                    {shipment.phoneNumber}
                                  </p>
                                </div>
                                <p className="text-gray-600">{shipment.address}</p>
                              </div>
                              <button
                                className="rounded-lg p-2 text-orange-500 hover:bg-orange-500 hover:text-white"
                                type="button"
                                onClick={() => openUpdateModal(shipment)}
                              >
                                {t("payment.update")}
                              </button>
                            </div>
                          ))}
                          <AddShipment
                          refreshShipments={refreshShipments}
                          />
                        </div>
                      )}
                    </Dialog.Panel>
                  </Transition.Child>
                </div>
              </div>
            </Dialog>
          </Transition>
        </div>
      )}
      {isUpdateModalOpen && (
        <UpdateShipment shipment={currentShipment} onClose={closeUpdateModal} />
      )}
    </>
  );
}
