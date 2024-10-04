import { Dialog, Transition } from "@headlessui/react";
import { Fragment, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { useDispatch } from "react-redux";
import { login } from "../../redux/slices/authSlice";
import { authenticateUser } from "../../services/authService";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

export default function EmployeeLoginModal({
  isOpen,
  closeModal,
  openSignInModal,
}) {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { t } = useTranslation("translation");
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const [showPassword, setShowPassword] = useState(false);

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const onSubmit = async (data) => {
    try {
      const decoded = await authenticateUser(dispatch, data);
      console.log(decoded);
      if (decoded.role === "Admin") {
        navigate("/admin/dashboard");
      } else if (decoded.role === "Employee") {
        navigate("/employee/warehouse");
      }
      toast.success(t("employee_login.success"));
      closeModal(); // Close the employee login modal
    } catch (error) {
      console.error("Employee login failed:", error);
      //   toast.error(t("employee_login.fail"));
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
              <Dialog.Panel className="flex justify-between w-1/2 transform overflow-hidden rounded-md shadow-xl transition-all">
                <div className="bg-white w-full px-10 text-black flex-col flex font-poppins justify-center py-10 space-y-4">
                  {/* New content for Admin/Employee information */}
                  <p className="font-poppins text-center text-gray-500">
                    {t("employee_login.info_message")}
                  </p>

                  <form
                    onSubmit={handleSubmit(onSubmit)}
                    className="space-y-3 text-black flex-col flex font-poppins justify-center"
                  >
                    <label className="font-alfa text-xl text-center mb-2">
                      {t("employee_login.signin")}
                    </label>
                    <input
                      type="text"
                      placeholder={t("employee_login.username_placeholder")}
                      className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                      {...register("userName", { required: true })}
                    />
                    {errors.userName && (
                      <p className="text-red-400 text-sm italic">
                        {t("employee_login.username_required")}
                      </p>
                    )}

                    <div className="relative">
                      <input
                        type={showPassword ? "text" : "password"}
                        placeholder={t("employee_login.password_placeholder")}
                        className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                        {...register("password", { required: true })}
                      />
                      <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
                        <FontAwesomeIcon
                          icon={showPassword ? faEyeSlash : faEye}
                          className="cursor-pointer text-orange-400"
                          onClick={togglePasswordVisibility}
                        />
                      </div>
                    </div>
                    {errors.password && (
                      <p className="text-red-400 text-sm italic">
                        {t("employee_login.password_required")}
                      </p>
                    )}

                    <button
                      type="submit"
                      className="bg-orange-500 font-alfa text-white rounded-lg px-10 py-2 w-full"
                    >
                      {t("employee_login.signin_button")}
                    </button>
                  </form>

                  {/* Back to regular sign-in button */}
                  <button
                    className="mt-4 text-blue-500 underline"
                    onClick={() => {
                      closeModal();
                      openSignInModal(); // Open regular sign-in modal
                    }}
                  >
                    {t("employee_login.back_to_signin")}
                  </button>
                </div>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
}
