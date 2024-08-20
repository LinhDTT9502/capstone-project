import {
  Button,
  Card,
  CardBody,
  Collapse,
  Select,
  Typography,
} from "@material-tailwind/react";
import React from "react";
import { useTranslation } from "react-i18next";
const ContactUs = () => {
  const { t } = useTranslation();
  // const [open, setOpen] = React.useState(false);

  // const toggleOpen = () => setOpen((cur) => !cur);
  return (
    <div className="relative mb-[4%]">
      <div className="flex justify-center relative">
        <img
          src="/assets/images/ContactUs.jpg"
          alt="contactUs"
          className="mx-auto h-[350px] w-[90%]"
        />
        <h1 className="text-[60px] py-20 text-white font-alfa absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2">
          {t("contactUs.txt")}
        </h1>
      </div>

      <div className="mt-10 mx-[10%] flex">
        <div className="w-1/3 justify-center flex flex-col ml-20 mr-20">
          <h2 className="text-[20px] font-bold text-[#524FF5]">
            {t("WELCOMETOSTRENGTHY.txt")}
          </h2>
          <h1 className="text-[35px] font-bold text-black">
            {t("GetInTouchWithUs.txt")}
          </h1>
          <p className="text-[15px] text-[#6A6A6A] mt-3 text-wrap">
            {t("AskFeedback.txt")}
          </p>
          <h2 className="text-[20px] font-bold text-black mt-3">
            {t("OpenHours.txt")}
          </h2>
          <div className="mt-3">
            <p className="text-[15px] text-[#6A6A6A] mt-2">
              {t("MontoFri.txt")}
            </p>
            <p className="text-[15px] text-[#6A6A6A] mt-2">{t("Sat.txt")}</p>
            <p className="text-[15px] text-[#6A6A6A] mt-2">{t("Sun.txt")}</p>
          </div>
        </div>

        <div className="w-2/3 bg-[#EEEEEE] mx-16">
          <h1 className="text-[35px] font-bold text-black mt-10 ml-[14%]">
            {t("SendUsaMessage.txt")}
          </h1>
          <h2 className="text-[15px] font-bold text-[#524FF5] ml-[14%]">
            {t("YourEmail.txt")}
          </h2>
          <div className="flex flex-col mt-7 items-center border-black">
            <input
              type="text"
              placeholder={t("YourFullName.txt")}
              className="bg-white h-12 w-[70%] text-black pl-5 rounded-lg"
            />
            <input
              type="email"
              placeholder="Email"
              className="bg-white mt-5 h-12 w-[70%] text-black pl-5 rounded-lg"
            />
            <input
              type="text"
              placeholder={t("Subject.txt")}
              className="bg-white mt-5 h-12 w-[70%] text-black pl-5 rounded-lg"
            />
            <input
              type="text"
              placeholder={t("Message.txt")}
              className="bg-white mt-5 h-24 w-[70%] text-black pl-5 rounded-lg"
            />
          </div>
          <Button className="mt-10 ml-[15%] mb-10"> {t("SendNow.txt")}</Button>
        </div>
      </div>

      <div className="flex flex-col justify-center mt-14 items-center mb-10 text-2xl font-bold mx-[20%]">
        <h2 className="text-[#524FF5] font-bold justify-center">FAQ</h2>
        <h1 className="text-[35px] font-bold text-black mt-5">
          {t("FrequentlyAskedQuestions.txt")}
        </h1>
        <div className="w-[50%] h-16 mt-5 text-2xl font-bold">
          <Select
            className="block border-0 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 rounded-md h-16  text-2xl font-bold"
            label={t("what.txt")}
          >
            <option value="">None</option>
          </Select>
          <div className="h-px bg-gray-300 mt-10 mb-10"></div>
        </div>

        {/* <div className="w-[50%] h-16 mt-10">
     <Button className="bg-[#FFFFFF] text-black h-16 w-[70%] ml-[15%]"
     onClick={toggleOpen}>Open Collapse</Button>
      <Collapse open={open}>
        <Card className="my-4 mx-auto w-8/12">
          <CardBody>
            <Typography>
              Use our Tailwind CSS collapse for your website. You can use if for
              accordion, collapsible items and much more.
            </Typography>
          </CardBody>
        </Card>
      </Collapse> 
     </div> */}

        <div className="w-[50%] h-16 mt-10">
          <Select
            className="block text-base border-0 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 rounded-md h-16 "
            label={t("DoIneed?.txt")}
          >
            <option value="" className="text-[#6A6A6A] text-wrap">
              Lorem Ipsum is simply dummy text of the printing and typesetting
              industry. Lorem Ipsum has been the industry's standard dummy text
              ever since the 1500s, when an unknown printer took a galley of
              type and scrambled it to make a type specimen book.{" "}
            </option>
          </Select>
          <div className="h-px bg-gray-300 mt-10"></div>
        </div>

        <div className="w-[50%] h-16 mt-10">
          <Select
            className="block text-base border-0 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 rounded-md h-16 "
            label={t("Doyouoffer.txt")}
          >
            <option value="" className="text-[#6A6A6A] text-wrap">
              Lorem Ipsum is simply dummy text of the printing and typesetting
              industry. Lorem Ipsum has been the industry's standard dummy text
              ever since the 1500s, when an unknown printer took a galley of
              type and scrambled it to make a type specimen book.{" "}
            </option>
          </Select>
          <div className="h-px bg-gray-300 mt-10"></div>
        </div>

        <div className="w-[50%] h-16 mt-10">
          <Select
            className="block text-base border-0 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 rounded-md h-16 "
            label={t("Doyouoffer.txt")}
          >
            <option value="" className="text-[#6A6A6A] text-wrap">
              Lorem Ipsum is simply dummy text of the printing and typesetting
              industry. Lorem Ipsum has been the industry's standard dummy text
              ever since the 1500s, when an unknown printer took a galley of
              type and scrambled it to make a type specimen book.{" "}
            </option>
          </Select>
          <div className="h-px bg-gray-300 mt-10"></div>
        </div>
      </div>
    </div>
  );
};

export default ContactUs;
