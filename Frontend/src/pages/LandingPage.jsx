import React from "react";
import Banner from "../components/Banner";
import Brands from "../components/Brands";
import Ads from "../components/Ads";
import WebServices from "../components/WebServices";
import FeatureProductSlide from "../components/Product/FeatureProductSlide";

function LandingPage() {

    return (
        <>
            <Banner />
            <Brands />
            <Ads />
            <FeatureProductSlide />
            <WebServices/>
        </>
    );
}

export default LandingPage;
