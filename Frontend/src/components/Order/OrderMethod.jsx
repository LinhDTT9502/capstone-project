import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import DeliveryAddress from "../Payment/DeliveryAddress";
import { fetchBranchs } from "../../services/branchService";
import { Avatar, Card, List, ListItem, ListItemPrefix, Typography, Radio } from "@material-tailwind/react";
import { fetchProductsbyBranch } from "../../services/warehouseService";

const OrderMethod = ({ userData, setUserData, selectedOption, handleOptionChange, selectedBranchId, setSelectedBranchId }) => {
    const { t } = useTranslation();
    const [branches, setBranches] = useState([]);
    const [branchStatus, setBranchStatus] = useState({});

    useEffect(() => {
        // Load branches and their availability statuses
        const loadBranchesWithStatus = async () => {
            try {
                const branchData = await fetchBranchs();
                setBranches(branchData);

                // Check availability for each branch
                const statusPromises = branchData.map(async (branch) => {
                    const products = await fetchProductsbyBranch(branch.id);
                    const isAvailable = products.some(product => product.availableQuantity > 0);
                    return { branchId: branch.id, status: isAvailable ? "Còn hàng" : "Hết hàng" };
                });

                // Wait for all availability checks to complete
                const statuses = await Promise.all(statusPromises);

                // Update branchStatus with the results
                const statusMap = {};
                statuses.forEach(({ branchId, status }) => {
                    statusMap[branchId] = status;
                });
                setBranchStatus(statusMap);
            } catch (error) {
                console.error("Error loading branches or availability:", error);
            }
        };

        loadBranchesWithStatus();
    }, []);

    const handleBranchChange = (branchId) => {
        setSelectedBranchId(branchId);
        console.log("Selected Branch ID:", branchId);
    };

    return (
        <div className="pl-20 py-10">
            <h3 className="text-2xl font-bold pt-1">Phương thức nhận hàng</h3>
            <div className="flex pt-3">
                <form className="bg-white p-6 rounded shadow-md w-full border border-black">
                    <div className="mb-4">
                        <label className="inline-flex items-center">
                            <input
                                type="radio"
                                name="option"
                                value="HOME_DELIVERY"
                                className="form-radio text-[#FA7D0B]"
                                onChange={handleOptionChange}
                            />
                            <span className="ml-2">Giao tận nơi</span>
                        </label>
                        {selectedOption === "HOME_DELIVERY" && (
                            <div className="text-sm text-black bg-gray-300 p-2 rounded text-wrap">
                                <DeliveryAddress userData={userData} setUserData={setUserData} />
                            </div>
                        )}
                    </div>
                    <div className="mb-4">
                        <label className="inline-flex items-center">
                            <input
                                type="radio"
                                name="option"
                                value="STORE_PICKUP"
                                className="form-radio text-[#FA7D0B]"
                                onChange={handleOptionChange}
                            />
                            <span className="ml-2">Nhận tại cửa hàng</span>
                        </label>
                        {selectedOption === "STORE_PICKUP" && (
                            <div className="mt-4 text-sm text-black bg-gray-300 p-2 rounded text-wrap">
                                <Card className="w-full">
                                    <List>
                                        {branches.map((branch) => (
                                            <ListItem
                                                key={branch.id}
                                                className={`cursor-pointer hover:bg-gray-100 ${branchStatus[branch.id] === "Hết hàng" ? "opacity-50 cursor-not-allowed" : ""}`}
                                                onClick={() => branchStatus[branch.id] !== "Hết hàng" && handleBranchChange(branch.id)}
                                            >
                                                <input
                                                    type="radio"
                                                    name="branch"
                                                    value={branch.id}
                                                    checked={selectedBranchId === branch.id}
                                                    onChange={() => handleBranchChange(branch.id)}
                                                    className="mr-2"
                                                    disabled={branchStatus[branch.id] === "Hết hàng"} 
                                                />
                                                <ListItemPrefix>
                                                    <Avatar
                                                        variant="circular"
                                                        alt={branch.branchName}
                                                        src={branch.imgAvatarPath}
                                                    />
                                                </ListItemPrefix>
                                                <div>
                                                    <Typography variant="h6" color="blue-gray">
                                                        {branch.branchName} - {branch.hotline}
                                                    </Typography>
                                                    <Typography variant="small" color="gray" className="font-normal">
                                                        {branch.location}
                                                    </Typography>
                                                    <Typography variant="small" color={branchStatus[branch.id] === "Còn hàng" ? "green" : "red"} className="font-normal">
                                                        {branchStatus[branch.id]}
                                                    </Typography>
                                                </div>
                                            </ListItem>
                                        ))}
                                    </List>
                                </Card>
                            </div>
                        )}
                    </div>
                </form>
            </div>
        </div>
    );
};

export default OrderMethod;
