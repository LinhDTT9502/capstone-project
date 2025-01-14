import React, { useEffect, useState, useRef } from "react";
import { Link, NavLink } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faMagnifyingGlass,
  faLocationDot,
  faPhone,
  faEnvelope,
  faCaretDown,
  faUser,
  faCartShopping,
  faBinoculars,
  faBell,
} from "@fortawesome/free-solid-svg-icons";
import GetCurrentLocation from "../components/GetCurrentLocation";
import { useTranslation } from "react-i18next";
import i18n from "i18next";
import { Switch } from "@headlessui/react";
import SignInModal from "../components/Auth/SignInModal";
import { motion, useScroll } from "framer-motion";
import { BreadcrumbsDefault } from "./BreadcrumbsDefault";
import SearchBar from "../components/Product/SearchBar";
import BranchSystem from "../components/BranchButton";
import { getUserCart } from "../services/cartService";
import { useCart } from "../components/Cart/CartContext";
import SearchOrderDropDown from "../components/Research/SearchOrderDropDown";
import {
  Menu,
  MenuHandler,
  MenuList,
  MenuItem,
  Button,
} from "@material-tailwind/react";
import useOrderNotification from "../hook/Notification";
import { useSelector } from "react-redux";
import { selectUser } from "../redux/slices/authSlice";
import { getNoti } from "../services/Notification/NotificationService";
import { useNavigate } from "react-router-dom";

function Header() {
  const { scrollYProgress } = useScroll();
  const { t } = useTranslation("translation");
  const [enabled, setEnabled] = useState(true);
  const { cartCount, setCartCount, reload } = useCart();
  const token = localStorage.getItem("token");
  const [openNoti, setOpenNoti] = useState(false);
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [noti, setNoti] = useState([]);
  const user = useSelector(selectUser);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const navigate = useNavigate();
  const [prevScrollY, setPrevScrollY] = useState(0);
  const [visible, setVisible] = useState(true);

  const [isPolicyDropdownOpen, setIsPolicyDropdownOpen] = useState(false);
  const dropdownRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsPolicyDropdownOpen(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  const getNotification = async () => {
    if (token) {
      const data = await getNoti(user.UserId, token);
      const sortedData = data.sort(
        (a, b) => new Date(b.createdAt) - new Date(a.createdAt)
      );
      setNoti(sortedData);
    }
  };

  useEffect(() => {
    getNotification();
  }, [user]);

  const handleLinkClick = () => {
    setIsPolicyDropdownOpen(false);
  };

  useEffect(() => {
    const handleScroll = () => {
      if (window.scrollY > prevScrollY) {
        // Scrolling down, hide the header
        setVisible(false);
      } else {
        // Scrolling up, show the header
        setVisible(true);
      }
      setPrevScrollY(window.scrollY);
    };

    window.addEventListener("scroll", handleScroll);
    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, [prevScrollY]);

  const fetchCartCount = async () => {
    const token = localStorage.getItem("token");
    if (token) {
      const cartData = await getUserCart(token);
      const totalItems = cartData.length
      setCartCount(totalItems);
    }
  };

  

  useEffect(() => {
    fetchCartCount();
    console.log(reload);
    
  }, [setCartCount, reload]);

  useOrderNotification((message) => {
    setNotifications((prev) => [...prev, message]); // Add new notification to the list
    setUnreadCount((prev) => prev + 1); // Increment unread count
    getNotification();
  });

  const handleNotiToggle = () => {
    setOpenNoti(!openNoti);

    if (!openNoti) {
      setUnreadCount(0); // Clear unread count when opening the menu
    }
  };

  const highlightNumbers = (notification) => {
    let message;
    let id;

    // Check if notification is an object or a string
    if (typeof notification === "object") {
      message = notification.message;
      id = notification.id;
    } else {
      message = notification; // Assume it's already the message string
    }

    return message.split(/(S-\d+|T-\d+)/).map((part, index) =>
      /(S-\d+|T-\d+)/.test(part) ? (
        <span
          key={index}
          className="font-bold text-orange-500"
          onClick={() => handleOpenModal(part, id)}
        >
          {part}
        </span>
      ) : (
        part
      )
    );
  };

  const handleOpenModal = async (orderCode, id) => {
    const numericOrderCode = orderCode.replace(/^[A-Za-z]-/, "");

    // Set the selectedOrder to the numeric part
    setSelectedOrder(numericOrderCode);
    const response = await fetch(
      `https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Notification/update-status?id=${id}&isRead=true`,
      {
        method: "PUT",
        headers: {
          accept: "*/*",
        },
      }
    );

    // Open the appropriate modal based on the prefix
    if (orderCode.startsWith("S-")) {
      navigate(`/manage-account/sale-order/${numericOrderCode}`);
    } else if (orderCode.startsWith("T-")) {
      navigate(`/manage-account/user-rental/${numericOrderCode}`);
    }

    getNotification();
  };

  const handleCloseModal = () => {
    setSelectedOrder(null);
    setModalOpen(false);
  };

  const changeLanguage = () => {
    const languageValue = enabled ? "eng" : "vie";
    i18n.changeLanguage(languageValue);
  };
  return (
    <>
      <div className="w-full relative z-50 pb-28">
        <div
          className={`fixed top-0 left-0 right-0 transition-all duration-300 ease-in-out ${visible ? "transform translate-y-0" : "transform -translate-y-full"
            }`}
        >
          {" "}
          <div className="bg-white/95 backdrop-blur-lg font-medium text-black flex justify-between items-center relative text-xs py-2 z-50">
            <div className="flex pl-20 items-center space-x-2">
              <Link to="/">
                <img
                  src="/assets/images/Logo.png"
                  alt="2Sport"
                  className="max-w-sm max-h-8 pr-3"
                />
              </Link>
              <FontAwesomeIcon icon={faLocationDot} />

              <GetCurrentLocation />
              <div className=" pl-5">
                <BranchSystem />
              </div>

              {/* <Switch
                                checked={enabled}
                                onChange={() => {
                                    setEnabled(!enabled);
                                    changeLanguage();
                                }}
                                className={`${enabled ? 'bg-orange-200' : 'bg-orange-500'
                                    }  relative inline-flex h-5 w-10 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors duration-200 ease-in-out focus:outline-none focus-visible:ring-2  focus-visible:ring-white/75`}
                            >
                                <span
                                    aria-hidden="true"
                                    className={`${enabled ? 'translate-x-0' : 'translate-x-5'
                                        } pointer-events-none inline-block h-4 w-4 transform rounded-full bg-white shadow-lg ring-0 transition duration-200 ease-in-out`}
                                />
                            </Switch>
                            <span className="text-orange-500">{enabled ? 'VI' : 'EN'}</span> */}
            </div>
            {/*search*/}
            <SearchBar />

            <div className="flex pr-20 items-center space-x-4">
              <p>
                <FontAwesomeIcon icon={faPhone} className="pr-1" />
                +84 338-581-571
              </p>
              <p>
                <FontAwesomeIcon icon={faEnvelope} className="pr-1" />
                2sportteam@gmail.com
              </p>
              {/* <select onChange={changeLanguage} className="text-orange-500">
                                <option value="eng">English</option>
                                <option value="vie">Vietnamese</option>
                            </select> */}
            </div>
          </div>
          <div className="bg-zinc-800/80 backdrop-blur-lg text-white  flex justify-between items-center text-base font-normal py-5 pr-20  z-50">
            <div className="flex space-x-10 pl-20 ">
              <Link
                to="/"
                className=" hover:text-orange-500 focus:text-orange-500"
              >
                {t("header.home")}
              </Link>
              <Link
                to="/product"
                className=" hover:text-orange-500 focus:text-orange-500"
              >
                {t("header.product")}
                {/* <FontAwesomeIcon icon={faCaretDown} className="pl-2" /> */}
              </Link>
              {/* <Link to="/">{t("header.blog")}</Link> */}
              <Link
                to="/about-us"
                className=" hover:text-orange-500 focus:text-orange-500"
              >
                {t("header.about")}
              </Link>
              <Link
                to="/contact-us"
                className=" hover:text-orange-500 focus:text-orange-500"
              >
                {t("header.contact")}
              </Link>
              <Link
                to="/blog"
                className=" hover:text-orange-500 focus:text-orange-500"
              >
                Blog
              </Link>
              <Link
                to="/refund-request"
                className=" hover:text-orange-500 focus:text-orange-500"
              >
                Hoàn tiền
              </Link>
              <div
                className="relative"
                ref={dropdownRef}
                onMouseEnter={() => setIsPolicyDropdownOpen(true)}
                onMouseLeave={() => setIsPolicyDropdownOpen(false)}
              >
                <button className="hover:text-orange-500 transition-colors duration-200">
                  Chính sách
                </button>
                {isPolicyDropdownOpen && (
                  <div className="absolute left-0 w-80 bg-white text-black shadow-lg rounded-md py-2 z-50">
                    <Link
                      to="/complaints-handling"
                      className="block px-4 py-2 hover:bg-gray-100 transition-colors duration-200"
                    >
                      Chính sách xử lý khiếu nại
                    </Link>
                    <Link
                      to="/returns-refunds"
                      className="block px-4 py-2 hover:bg-gray-100 transition-colors duration-200"
                    >
                      Chính sách đổi trả, hoàn tiền
                    </Link>
                    <Link
                      to="/payment"
                      className="block px-4 py-2 hover:bg-gray-100 transition-colors duration-200"
                    >
                      Chính sách thanh toán
                    </Link>
                    <Link
                      to="/privacy"
                      className="block px-4 py-2 hover:bg-gray-100 transition-colors duration-200"
                    >
                      Chính sách bảo mật thông tin khách hàng
                    </Link>
                    <Link
                      to="/membership"
                      className="block px-4 py-2 hover:bg-gray-100 transition-colors duration-200"
                    >
                      Chính sách dành cho membership khi thuê đồ tại 2Sport
                    </Link>
                    <Link
                      to="/second-hand-rentals"
                      className="block px-4 py-2 hover:bg-gray-100 transition-colors duration-200"
                    >
                      Chính sách khi thuê đồ 2hand tại 2Sport
                    </Link>
                    <Link
                      to="/shipping"
                      className="block px-4 py-2 hover:bg-gray-100 transition-colors duration-200"
                    >
                      Chính sách vận chuyển
                    </Link>
                  </div>
                )}
              </div>
            </div>
            <div className="flex space-x-4 justify-center items-center">
              <SearchOrderDropDown />

              <SignInModal />
              <Link to="/cart" className="flex space-x-2">
                <div className="relative">
                  <FontAwesomeIcon icon={faCartShopping} className="pr-1" />
                  {cartCount > 0 && (
                    <span className="absolute -top-2 -right-2 bg-red-500 text-white text-[0.625rem] font-bold rounded-full h-[1rem] w-4 leading-none flex items-center justify-center">
                      {cartCount}
                    </span>
                  )}
                </div>
                <p>Giỏ hàng</p>
              </Link>

              {token && (
                <>
                  <Menu open={openNoti} handler={setOpenNoti}>
                    <MenuHandler>
                      <div
                        className="relative flex items-center justify-center cursor-pointer bg-white hover:bg-orange-500 p-2 rounded-full transition-colors duration-200"
                        onClick={handleNotiToggle}
                      >
                        <FontAwesomeIcon
                          icon={faBell}
                          className="text-xl text-orange-600 hover:text-white"
                        />
                        {unreadCount > 0 && (
                          <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                            {unreadCount}
                          </span>
                        )}
                      </div>
                    </MenuHandler>
                    <MenuList className="max-h-[40vh] overflow-y-auto bg-white shadow-xl rounded-md divide-y divide-gray-200 w-80">
                      {notifications.length === 0 && noti.length === 0 ? (
                        <MenuItem className="py-4 px-6 text-gray-500 italic">
                          Chưa có thông báo mới
                        </MenuItem>
                      ) : (
                        <>
                          {notifications.map((notification, index) => (
                            <MenuItem
                              key={`notification-${index}`}
                              className="hover:bg-orange-500 transition-colors duration-150 ease-in-out"
                            >
                              <p className="text-sm py-3 px-4">
                                {highlightNumbers(notification)}
                              </p>
                            </MenuItem>
                          ))}
                          {noti.map((notiItem) => (
                            <MenuItem
                              key={`noti-${notiItem.id}`}
                              className={`
                  ${notiItem.isRead ? "bg-white" : "bg-blue-50"} 
                  hover:bg-gray-200 transition-colors duration-150 ease-in-out
                `}
                            // onClick={(event) => handleNotificationClick(notiItem.id, event)}
                            >
                              <div className="flex items-center py-3 px-4">
                                {!notiItem.isRead && (
                                  <span className="w-2 h-2 bg-blue-500 rounded-full mr-3 flex-shrink-0"></span>
                                )}
                                <p
                                  className={`text-sm ${notiItem.isRead
                                      ? "text-gray-600"
                                      : "text-gray-800 font-medium"
                                    }`}
                                >
                                  {highlightNumbers(notiItem)}
                                </p>
                              </div>
                            </MenuItem>
                          ))}
                        </>
                      )}
                    </MenuList>
                  </Menu>
                </>
              )}
            </div>
          </div>
          <motion.div
            className="progress-bar"
            style={{ scaleX: scrollYProgress, zIndex: "-99999" }}
          />
          {/* <BreadcrumbsDefault/> */}
        </div>
      </div>
    </>
  );
}

export default Header;
