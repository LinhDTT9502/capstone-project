import { Dialog, Transition } from '@headlessui/react';
import { Fragment, useState } from 'react';
import { useForm } from 'react-hook-form';
import axios from 'axios';
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { useTranslation } from "react-i18next"; 

export default function ForgotPasswordModal({ isOpen, closeModal }) {
    const { t } = useTranslation();
  const { register, handleSubmit, formState: { errors } } = useForm();

  const onSubmit = async (data) => {
    try {
      const response = await axios.post('https://localhost:7276/api/Auth/forgot-password', data);
      toast.success("Liên kết đặt lại mật khẩu được gửi đến email của bạn!");
      closeModal();
    } catch (error) {
      toast.error("Không nhận được liên kết đặt lại mật khẩu. Vui lòng thử lại.");
    }
  };

  return (
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
          <div className="fixed inset-0 bg-black/25" />
        </Transition.Child>

        <div className="fixed inset-0 overflow-y-auto">
          <div className="flex min-h-full items-center justify-center pt-20">
            <Transition.Child
              as={Fragment}
              enter="ease-out duration-300"
              enterFrom="opacity-0 scale-95"
              enterTo="opacity-100 scale-100"
              leave="ease-in duration-200"
              leaveFrom="opacity-100 scale-100"
              leaveTo="opacity-0 scale-95"
            >
              <Dialog.Panel className="w-1/2 transform overflow-hidden rounded-md shadow-xl transition-all bg-white p-6">
                <h2 className="text-2xl font-semibold mb-4">{t("forgot_password.forgot_password")}</h2>
                <form onSubmit={handleSubmit(onSubmit)}>
                  <label className="block mb-2">{t("forgot_password.username")}</label>
                  <input
                    type="text"
                    placeholder={t("forgot_password.enter_username")}
                    className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full mb-4"
                    {...register('username', { required: true })}
                  />
                  {errors.username && <p className="text-red-400 text-sm italic">{t("forgot_password.required")}</p>}

                  <label className="block mb-2">{t("forgot_password.email")}</label>
                  <input
                    type="email"
                    placeholder={t("forgot_password.enter_email")}
                    className="text-gray-7000 p-2 rounded-lg border-2 border-zinc-400 w-full mb-4"
                    {...register('email', { required: true })}
                  />
                  {errors.email && <p className="text-red-400 text-sm italic">{t("forgot_password.required")}</p>}

                  <button type="submit" className="bg-orange-500 text-white rounded-lg px-4 py-2 mt-4 w-full">
                    {t("forgot_password.btn")}
                  </button>
                </form>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
}
