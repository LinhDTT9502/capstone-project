import { Dialog, Transition } from "@headlessui/react";
import { Fragment, useState } from "react";
import { useDispatch } from "react-redux";
import { login } from "../../redux/slices/authSlice";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import LoginGoogle from "./LoginGoogle";
import { signUpUser } from "../../services/authService";
import { useTranslation } from "react-i18next";

export default function SignUpModal({ isOpen, closeModal, openSignInModal }) {
  const { t } = useTranslation();
  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm();
  const [showPassword, setShowPassword] = useState(false);
  const dispatch = useDispatch();

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const onSubmit = async (data) => {
    const { username, password, fullName, email } = data;

    try {
      const response = await signUpUser({
        username,
        password,
        fullName,
        email,
      });
      console.log("Sign-up successful:", response);
      toast.success("Đăng ký thành công");
      // dispatch(login(response.user)); // Uncomment and adjust if needed
      closeModal();
    } catch (error) {
      console.error("Sign-up failed:", error);
    }
  };

  const handleSignInClick = () => {
    closeModal();
    openSignInModal();
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
              <Dialog.Panel className="flex justify-between w-3/5 transform overflow-hidden rounded-md shadow-xl transition-all">
                <div className="flex-col flex px-14 space-y-5 bg-zinc-700 text-white items-center justify-center w-1/2">
                  <h1 className="font-alfa text-lg text-orange-500">
                   {t("SignUpModal.Come join us!")}
                  </h1>
                  <p className="font-poppins text-center">
                {t("SignUpModal.welcome_message")}
                  </p>
                  <button
                    className="flex font-poppins bg-gradient-to-r from-zinc-500 to-zinc-600 w-fit p-3 shadow-zinc-800 shadow-md rounded-md"
                    onClick={handleSignInClick}
                  >
                   {t("SignUpModal.have_account")}{" "}
                    <p className="pl-1 text-orange-500 font-bold">{t("SignUpModal.Signin!")}</p>
                  </button>
                </div>
                <div className="bg-white w-1/2 px-20 text-black flex-col flex font-poppins justify-center py-10">
                  <form
                    onSubmit={handleSubmit(onSubmit)}
                    className=" space-y-3 text-black flex-col flex font-poppins justify-center"
                  >
                    <label className="font-alfa text-xl items-center text-center mb-2">
                    {t("SignUpModal.Signup")}
                    </label>
                    <input
                      type="text"
                      placeholder=  {t("SignUpModal.placeholder_fullname")}
                      className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                      {...register("fullName", { required: true })}
                    />
                    {errors.fullName && (
                      <p className="text-red-400 text-sm italic">
                      {t("SignUpModal.fullname")}
                      </p>
                    )}
                    <input
                      type="text"
                      placeholder={t("SignUpModal.placeholder_username")}
                      className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                      {...register("username", {
                        required: true,
                        maxLength: 20,
                        pattern: /^[a-zA-Z0-9_]+$/,
                      })}
                    />
                    {errors.username && errors.username.type === "required" && (
                      <p className="text-red-400 text-sm italic">
                       {t("SignUpModal.required")}
                      </p>
                    )}
                    {errors.username &&
                      errors.username.type === "maxLength" && (
                        <p>Username cannot exceed 20 characters</p>
                      )}
                    {errors.username && errors.username.type === "pattern" && (
                      <p className="text-red-400 text-sm italic">
                     {t("SignUpModal.validate_username")}
                      </p>
                    )}
                    <input
                      type="email"
                      placeholder={t("SignUpModal.placeholder_email")}
                      className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                      {...register("email", {
                        required: "Email is required",
                        pattern: {
                          value: /^\S+@\S+$/i,
                          message: "Invalid email address",
                        },
                      })}
                    />
                    {errors.email && (
                      <p className="text-red-400 text-sm italic">
                        {errors.email.message}
                      </p>
                    )}
                    <div className="relative">
                      <input
                        type={showPassword ? "text" : "password"}
                        placeholder=    {t("SignUpModal.placeholder_password")}
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
                      {t("SignUpModal.validate_password")}
                      </p>
                    )}
                    <div className="relative">
                      <input
                        type={showPassword ? "text" : "password"}
                        placeholder= {t("SignUpModal.placeholder_confirmPassword")}
                        className="text-gray-700 p-2 rounded-lg border-2 border-zinc-400 w-full"
                        {...register("confirmPassword", {
                          required: true,
                          validate: (value) => value === watch("password"),
                        })}
                      />
                      <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
                        <FontAwesomeIcon
                          icon={showPassword ? faEyeSlash : faEye}
                          className="cursor-pointer text-orange-400"
                          onClick={togglePasswordVisibility}
                        />
                      </div>
                    </div>
                    {errors.confirmPassword &&
                      errors.confirmPassword.type === "required" && (
                        <p className="text-red-400 text-sm italic">
                   {t("SignUpModal.validate_confirmPassword")}
                        </p>
                      )}
                    {errors.confirmPassword &&
                      errors.confirmPassword.type === "validate" && (
                        <p className="text-red-400 text-sm italic">
                   {t("SignUpModal.notmatch_confirmPassword")}
                        </p>
                      )}
                    <button
                      type="submit"
                      className="bg-orange-500 font-alfa text-white rounded-lg px-10 py-2 w-full"
                    >
                          {t("SignUpModal.Signup")}
                    </button>
                  </form>
                  <LoginGoogle />
                </div>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </div>
      </Dialog>
    </Transition>
  );
}
