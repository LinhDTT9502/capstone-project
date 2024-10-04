import { Dialog, Transition } from "@headlessui/react";
import { Fragment, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUser, faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";
import { useTranslation } from "react-i18next";
import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { login, selectUser } from "../../redux/slices/authSlice";
import UserDropdown from "../User/userDropdown";
import SignUpModal from "./SIgnUpModal";
import LoginGoogle from "./LoginGoogle";
import ForgotPasswordModal from "./ForgotPasswordModal";
import EmployeeLoginModal from "../Auth/SignInEmployeeModal"; // Import employee login modal
import { authenticateUser } from "../../services/authService";
import { useNavigate } from "react-router-dom";

export default function SignInModal() {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const user = useSelector(selectUser);
  const { t } = useTranslation("translation");
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const [isSignInOpen, setIsSignInOpen] = useState(false);
  const [isSignUpOpen, setIsSignUpOpen] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [isForgotPasswordOpen, setIsForgotPasswordOpen] = useState(false);
  const [isEmployeeLoginOpen, setIsEmployeeLoginOpen] = useState(false); // Employee login state

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const onSubmit = async (data) => {
    try {
      const decoded = await authenticateUser(dispatch, data);
      setIsSignInOpen(false);
      if (decoded.role === "Admin") {
        navigate("/admin/dashboard");
      } else if (decoded.role === "Employee") {
        navigate("/employee/warehouse");
      }
    } catch (error) {
      // Handle error inside authenticateUser
      console.error("User login failed:", error);
    }
  };

  const openEmployeeLoginModal = () => {
    setIsEmployeeLoginOpen(true);
    setIsSignInOpen(false); // Close sign-in modal when employee login modal opens
  };

  const closeEmployeeLoginModal = () => {
    setIsEmployeeLoginOpen(false);
  };

  return (
    <>
      <div>
        {user ? (
          <UserDropdown />
        ) : (
          <button
            type="button"
            onClick={() => setIsSignInOpen(true)}
            className="border-r-2 pr-4"
          >
            <FontAwesomeIcon icon={faUser} className="pr-1" />{" "}
            {t("header.signin")}
          </button>
        )}
      </div>

      {/* Sign In Modal */}
      <Transition appear show={isSignInOpen} as={Fragment}>
        <Dialog as="div" className="relative z-10" onClose={() => setIsSignInOpen(false)}>
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
                  <div className="flex-col flex py-20 px-10 space-y-5 bg-zinc-700 text-white items-center w-1/2">
                    <h1 className="font-alfa text-lg text-orange-500">
                      {t("signin.welcome_back")}
                    </h1>
                    <p className="font-poppins text-center">
                      {t("signin.welcome_back_message")}
                    </p>
                    <button
                      className="flex font-poppins bg-gradient-to-r from-zinc-500 to-zinc-600 w-fit p-3 shadow-zinc-800 shadow-md rounded-md"
                      onClick={() => {
                        setIsSignInOpen(false);
                        setIsSignUpOpen(true);
                      }}
                    >
                      {t("signin.no_account")}
                      <p className="pl-1 text-orange-500 font-bold">
                        {t("signin.signup")}
                      </p>
                    </button>
                    <button
                      className="flex font-poppins bg-gradient-to-r from-zinc-500 to-zinc-600 w-fit p-3 shadow-zinc-800 shadow-md rounded-md mt-3"
                      onClick={openEmployeeLoginModal} // Open employee login modal
                    >
                      {t("signin.signin_employee")}
                    </button>
                  </div>
                  <div className="bg-white w-1/2 px-20 text-black flex-col flex font-poppins justify-center">
                    <form
                      onSubmit={handleSubmit(onSubmit)}
                      className="text-black flex-col flex font-poppins justify-center pb-5"
                    >
                      {/* Username Input */}
                      <label className="">{t("signin.username")}</label>
                      <input
                        type="text"
                        placeholder={t("signin.enter_username")}
                        className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                        {...register("userName", {
                          required: true,
                          maxLength: 20,
                          pattern: /^[a-zA-Z0-9_]+$/,
                        })}
                      />
                      {errors.userName && (
                        <p className="text-red-400 text-sm italic">
                          {t("signin.required")}
                        </p>
                      )}

                      {/* Password Input */}
                      <label className="">{t("signin.password")}</label>
                      <div className="relative">
                        <input
                          type={showPassword ? "text" : "password"}
                          placeholder={t("signin.enter_password")}
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
                          {t("signin.password_required")}
                        </p>
                      )}

                      {/* Forgot Password */}
                      <label
                        className="text-left pb-3 text-blue-500 underline cursor-pointer"
                        onClick={() => {
                          setIsSignInOpen(false);
                          setIsForgotPasswordOpen(true);
                        }}
                      >
                        {t("signin.forgot_password")}
                      </label>

                      {/* Submit Button */}
                      <button
                        type="submit"
                        className="bg-orange-500 font-alfa text-white rounded-lg px-10 py-2 w-full"
                        onClick={() => setIsSignInOpen(false)}
                      >
                        {t("signin.signin")}
                      </button>
                    </form>

                    <LoginGoogle setIsSignInOpen={setIsSignInOpen} />
                  </div>
                </Dialog.Panel>
              </Transition.Child>
            </div>
          </div>
        </Dialog>
      </Transition>

      {/* Sign Up Modal */}
      <SignUpModal
        isOpen={isSignUpOpen}
        closeModal={() => setIsSignUpOpen(false)}
        openSignInModal={() => setIsSignInOpen(true)}
      />

      {/* Employee Login Modal */}
      <EmployeeLoginModal
        isOpen={isEmployeeLoginOpen}
        closeModal={closeEmployeeLoginModal}
        openSignInModal={() => setIsSignInOpen(true)} // Add this to pass the method for back navigation
      />

      {/* Forgot Password Modal */}
      <ForgotPasswordModal
        isOpen={isForgotPasswordOpen}
        closeModal={() => setIsForgotPasswordOpen(false)}
      />
    </>
  );
}