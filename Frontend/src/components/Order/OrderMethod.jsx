import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import DeliveryAddress from "../Payment/DeliveryAddress";
import { fetchBranchs } from "../../services/branchService";
import { Avatar, Card, List, ListItem, ListItemPrefix, Typography, Radio } from "@material-tailwind/react";
import { fetchProductsbyBranch } from "../../services/warehouseService";
import AddressForm from "../AddressForm";
import { useSelector } from "react-redux";
import { selectUser } from "../../redux/slices/authSlice";

const OrderMethod = ({ userData, setUserData, selectedOption, handleOptionChange, selectedBranchId, setSelectedBranchId, selectedProducts }) => {
    const { t } = useTranslation();
    const [branches, setBranches] = useState([]);
    const [branchStatus, setBranchStatus] = useState({});
    const user = useSelector(selectUser);
    const [check, setCheck] = useState([]);
    console.log(selectedProducts);


    useEffect(() => {
        // Load branches and their availability statuses
        const loadBranchesWithStatus = async () => {
            try {
                const branchData = await fetchBranchs();
                setBranches(branchData);

                const statusPromises = branchData.map(async (branch) => {
                    const products = await fetchProductsbyBranch(branch.id);
                    // Track unavailable products for each branch
                    const unavailableProducts = selectedProducts.map((selectedProduct) => {
                        // Find the branch product by matching selected product id
                        const branchProduct = products.find((p) => p.productId === Number(selectedProduct.id)); // Use Number to ensure type consistency
      
                        // If the branch product exists and selected product quantity exceeds available quantity
                        if (branchProduct && selectedProduct.quantity > branchProduct.availableQuantity) {
         
                            return {
                                productName: selectedProduct.productName,
                                productId: selectedProduct.id,
                                availableQuantity: branchProduct.availableQuantity,
                            };
                            
                            
                        }

                        // If product is available or not selected for out of stock, return null
                        return null;
                    }).filter(product => product !== null); // Filter out null values

                    // Determine branch status
                    const isAvailable = unavailableProducts.length === 0;
                   setCheck(unavailableProducts)
                    return {
                        branchId: branch.id,
                        status: isAvailable
                            ? "Còn hàng" // In stock
                            : `Hết hàng: ${unavailableProducts.map(p => `${p.productName} (Số lượng còn: ${p.availableQuantity})`).join(", ")}`, // Out of stock
                    };
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
    }, [selectedProducts]);


    const handleBranchChange = (branchId) => {
        setSelectedBranchId(branchId);
        console.log("Selected Branch ID:", branchId);
    };
    const handleAddressChange = (fullAddress) => {
        setUserData((prevData) => ({ ...prevData, address: fullAddress }));
    };
    return (
        <div className="pl-20 py-10">
            <div className="flex pt-3">
                <form className="bg-white w-full">
                    <div className="mb-4">
                        <h3 className="text-xl font-bold">Thông tin khách hàng</h3>

                        <DeliveryAddress userData={userData} setUserData={setUserData} />
                        <h3 className="text-xl font-bold pt-4">Phương thức nhận hàng</h3>
                        <label className="inline-flex items-center pt-4">
                            <input
                                type="radio"
                                name="option"
                                value="HOME_DELIVERY"
                                className="form-radio text-[#FA7D0B]"
                                onChange={handleOptionChange}
                            />
                            <span className="ml-2">Giao tận nơi</span>
                        </label>
                        {/* {selectedOption === "HOME_DELIVERY" && (<>
                            {!user && <div className="text-sm text-black bg-gray-300 p-2 rounded text-wrap">
                              
                                <AddressForm onAddressChange={handleAddressChange} />
                            </div>
                            }
                        </>

                        )} */}
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
                            <div className="max-h-[50vh] overflow-y-auto mt-4 text-sm text-black bg-gray-300 p-2 rounded text-wrap">
                                <Card className="w-full">
                                    <List>
                                        {branches.map((branch) => (
                                            <ListItem
                                            disabled={branchStatus[branch.id] !== "Còn hàng"}
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
                                                    disabled={branchStatus[branch.id] !== "Còn hàng"}
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
