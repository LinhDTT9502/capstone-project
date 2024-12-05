import React from "react";
import Banner from "../components/Banner";
import Brands from "../components/Brands";
import Ads from "../components/Ads";
import WebServices from "../components/WebServices";
import FeatureProductSlide from "../components/Product/FeatureProductSlide";
import SaleProductSlide from "../components/Product/SaleProductSlide";
import Categories from "../components/Categories";

function LandingPage() {

    return (
        <>
            <Banner />
            <Categories />
            <Brands />
            <Ads />
            <FeatureProductSlide />
            <SaleProductSlide/>
            
            <WebServices/>
        </>
    );
}

export default LandingPage;
