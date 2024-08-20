import React, { useEffect, useState } from "react";

function GetCurrentLocation(){
    const [add,setAdd] = useState('')
    // `https://nominatim.openstreetmap.org/reverse?format=json&lat=${latitude}&lon=${longitude}`
    
    useEffect(()=>{
        navigator.geolocation.getCurrentPosition(pos=>{
            const {latitude,longitude} = pos.coords;
            // console.log(latitude,longitude)
            const url = `https://nominatim.openstreetmap.org/reverse?format=json&lat=${latitude}&lon=${longitude}`;
            fetch(url).then(res=>res.json()).then(data=>setAdd(data.address))
        })
    },[])
    // console.log(add,"sfsfh")
    return(
        <>
            <p>{add.city}, {add.country}</p>
        </>
    )
}

export default GetCurrentLocation;