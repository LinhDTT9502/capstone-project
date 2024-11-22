import {
    Menu,
    MenuHandler,
    MenuList,
    MenuItem,
    Typography,
    
} from "@material-tailwind/react";
import { ProductColor } from "./ProductColor";
import { ProductSize } from "./ProductSize";
import { ProductCondition } from "./ProductCondition";

export function ProductType({productCode, color, size, condition}) {
    
    return (
        <Menu>
            <MenuHandler>
                <Typography variant="text">Phân loại hàng:</Typography>
            </MenuHandler>
            <MenuList>
                <MenuItem>
                    <ProductColor productCode={productCode}/>
                </MenuItem>
                <MenuItem>
                    <ProductSize 
                     productCode={productCode}
                     color={color}/>
                </MenuItem>
                <MenuItem>
                    <ProductCondition
                    productCode={productCode}
                    color={size}
                    size={condition} />
                </MenuItem>
            </MenuList>
        </Menu>
    );
}