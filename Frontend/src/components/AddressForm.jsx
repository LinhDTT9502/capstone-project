import React, { useState, useEffect } from "react";
import axios from "axios";
import { useTranslation } from "react-i18next";

const AddressForm = ({ onAddressChange }) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState({
        city: "",
        district: "",
        ward: "",
        street: ""
    });

    const [data, setData] = useState([]);
    const [districts, setDistricts] = useState([]);
    const [wards, setWards] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            const result = await axios({
                url: "https://raw.githubusercontent.com/kenzouno1/DiaGioiHanhChinhVN/master/data.json",
                method: "GET",
            });
            setData(result.data);
        };
        fetchData();
    }, []);

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setFormData({ ...formData, [name]: value });

        if (name === "city") {
            const selectedCity = data.find(city => city.Id === value);
            setDistricts(selectedCity ? selectedCity.Districts : []);
            setWards([]);
        } else if (name === "district") {
            const selectedDistrict = districts.find(district => district.Id === value);
            setWards(selectedDistrict ? selectedDistrict.Wards : []);
        }
    };

    const getAddressString = () => {
        const selectedCity = data.find(city => city.Id === formData.city);
        const selectedDistrict = districts.find(district => district.Id === formData.district);
        const selectedWard = wards.find(ward => ward.Id === formData.ward);

        const cityName = selectedCity ? selectedCity.Name : "";
        const districtName = selectedDistrict ? selectedDistrict.Name : "";
        const wardName = selectedWard ? selectedWard.Name : "";

        return `${formData.street}, ${wardName}, ${districtName}, ${cityName}`;
    };

    useEffect(() => {
        onAddressChange(getAddressString());
        console.log(getAddressString());
    }, [formData]);

    return (
        <div>
            <input
                type="text"
                name="street"
                value={formData.street}
                onChange={handleInputChange}
                className="mt-1 block w-full border rounded-md p-2 focus:ring-blue-500 focus:border-blue-500"
            />
            
            <select
                className="form-select form-select-sm mb-3"
                name="city"
                value={formData.city}
                onChange={handleInputChange}
            >
                <option value="">{t("address_form.select_city")}</option>
                {data.map(city => (
                    <option key={city.Id} value={city.Id}>
                        {city.Name}
                    </option>
                ))}
            </select>

            <select
                className="form-select form-select-sm mb-3"
                name="district"
                value={formData.district}
                onChange={handleInputChange}
            >
                <option value="">{t("address_form.select_district")}</option>
                {districts.map(district => (
                    <option key={district.Id} value={district.Id}>
                        {district.Name}
                    </option>
                ))}
            </select>

            <select
                className="form-select form-select-sm"
                name="ward"
                value={formData.ward}
                onChange={handleInputChange}
            >
                <option value="">{t("address_form.select_ward")}</option>
                {wards.map(ward => (
                    <option key={ward.Id} value={ward.Id}>
                        {ward.Name}
                    </option>
                ))}
            </select>
        </div>
    );
};

export default AddressForm;
