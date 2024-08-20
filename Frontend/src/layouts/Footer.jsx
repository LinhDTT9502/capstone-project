import { faFacebook, faInstagram } from "@fortawesome/free-brands-svg-icons";
import {
  faEnvelope,
  faLocationDot,
  faMapLocationDot,
  faPhone,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Typography } from "@material-tailwind/react";
import React from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

function Footer() {
  const { t } = useTranslation();
  return (
    <div className="bg-zinc-700 h-full">
      <div className="bg-footer bg-cover bg-center flex justify-center items-center">
        <h1 className="text-3xl py-20 text-white font-alfa drop-shadow-md">
          {t("footer.text-img")}
          <Link
            to="https://fb.com/profile.php?id=61560697567321"
            className="underline underline-offset-2 pl-8"
          >
            Facebook
          </Link>
        </h1>
      </div>

      <div className="flex justify-between items-center py-10 space-x-10 ">
        {/* left side */}
        <div className="px-20 space-y-8 w-1/2 text-white ">
          <img
            src="/assets/images/Logo.png"
            alt="2Sport"
            className="max-w-sm max-h-12"
          />
          <Typography>{t("footer.text")}</Typography>
          <Typography> {t("footer.Copyright")}</Typography>
          <div>
            <p className="font-alfa text-xl pb-4 ">{t("footer.getupdate")}</p>
            <div className="flex w-full bg-white ">
              <input
                className="flex-grow bg-transparent outline-none placeholder-gray-400 font-poppins pl-5"
                placeholder={t("footer.input")}
                type="text"
              />
              <button className="bg-orange-500 px-12 py-4 font-poppins">
                {t("footer.btn")}
              </button>
            </div>
          </div>
        </div>
        {/* right side */}
        <div className="w-1/2">
          {/* <div className="grid grid-cols-1 md:grid-cols-3">
            <div className="space-y-2 text-white">
              <Typography className="text-white mb-4 font-alfa">
                Company
              </Typography>
              <Typography className=" ">Liên hệ</Typography>
              <Typography className=" ">Testimonials</Typography>
              <Typography className="">FAQS</Typography>
              <Typography className="">Terms & Condition</Typography>
              <Typography className=" ">Latest Update</Typography>
            </div>

            <div className="space-y-2 text-white ">
              <Typography className="text-white font-alfa">
                Products
              </Typography>
              <Typography className=" ">Men's Section</Typography>
              <Typography className=" ">Women's Section</Typography>
              <Typography className="">Kid's Section</Typography>
              <Typography className=" ">Shoes</Typography>
              <Typography className="">Apparel</Typography>
              <Typography className=" ">Equipments</Typography>
            </div>

            <div className="space-y-2 text-white ">
              <Typography className="text-white font-alfa">
                Support
              </Typography>
              <Typography className=" ">Order Tracking</Typography>
              <Typography className=" ">Payment Guide</Typography>
              <Typography className="">Help Centre</Typography>
              <Typography className="">Privacy Policy</Typography>
              <Typography className="">Return Policy</Typography>
              <Typography className="">Promo Codes</Typography>
            </div>
          </div> */}
          <div className="grid grid-cols-3 pt-6">
            <div className="space-y-2 col-span-2 text-white">
              <Typography className="">
                <FontAwesomeIcon icon={faLocationDot} className="pr-1" />
                {t("footer.address")}
              </Typography>
              <Typography className="">
                <FontAwesomeIcon icon={faPhone} className="pr-1" />
                +84 338-581-571
              </Typography>
              <Typography className="">
                <FontAwesomeIcon icon={faEnvelope} className="pr-1" />
                2sportteam@gmail.com
              </Typography>
            </div>

            <div className="space-y-2 text-white">
              <Typography className="text-white font-alfa">
                {t("footer.contact")}
              </Typography>
              <Typography className="">
                <a
                  href="https://www.facebook.com/profile.php?id=61560697567321&mibextid=LQQJ4d"
                  className="text-white"
                >
                  <FontAwesomeIcon icon={faFacebook} className="pr-1" />
                  Facebook
                </a>
              </Typography>
              <Typography className="">
                <a href="https://www.instagram.com/2sport_tt/">
                  <FontAwesomeIcon icon={faInstagram} className="pr-1" />
                </a>
                Instagram
              </Typography>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Footer;
