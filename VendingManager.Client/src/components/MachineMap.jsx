import React from 'react';
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import { renderToStaticMarkup } from "react-dom/server";
import { MapPin } from "lucide-react";

// Helper do ikon (przeniesiony z App.jsx)
const getCustomIcon = (status, isDarkMode) => {
  const color =
    status === "Online"
      ? isDarkMode
        ? "#4ade80"
        : "#16a34a"
      : isDarkMode
      ? "#f87171"
      : "#dc2626";

  const iconMarkup = renderToStaticMarkup(
    <div
      className={`relative flex items-center justify-center w-10 h-10 custom-pin ${
        status === "Online" ? "custom-pin-online" : "custom-pin-offline"
      }`}
    >
      <MapPin
        size={40}
        fill={isDarkMode ? color : "white"}
        color={color}
        strokeWidth={2}
        className="drop-shadow-md"
      />
      <div
        className="absolute top-3 w-3 h-3 rounded-full bg-white"
        style={{ backgroundColor: isDarkMode ? "#000" : color }}
      ></div>
    </div>
  );

  return L.divIcon({
    html: iconMarkup,
    className: "bg-transparent border-none",
    iconSize: [40, 40],
    iconAnchor: [20, 40],
    popupAnchor: [0, -40],
  });
};

const MachineMap = ({ machines, isDarkMode, onMachineSelect }) => {
  return (
    <div className="bg-white dark:bg-neutral-900 rounded-3xl shadow-lg border border-gray-100 dark:border-neutral-800 overflow-hidden relative z-0 h-[400px] transition-colors duration-300">
      <MapContainer
        center={[53.1325, 23.1688]}
        zoom={13}
        style={{ height: "100%", width: "100%" }}
      >
        <TileLayer
          url={
            isDarkMode
              ? "https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png"
              : "https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png"
          }
          attribution="&copy; CARTO"
        />
        {machines.map((m) => {
          const lat = m.latitude || m.Latitude;
          const lon = m.longitude || m.Longitude;
          if (!lat || !lon) return null;

          return (
            <Marker
              key={m.id}
              position={[lat, lon]}
              icon={getCustomIcon(m.status, isDarkMode)}
            >
              <Popup className="custom-popup">
                <div className="text-center p-1">
                  <strong className="block text-lg mb-1 text-gray-900 dark:text-white">
                    {m.name}
                  </strong>
                  <button
                    className="bg-blue-600 text-white px-3 py-1 rounded-md text-sm hover:bg-blue-700"
                    onClick={() => onMachineSelect(m)}
                  >
                    Otwórz
                  </button>
                </div>
              </Popup>
            </Marker>
          );
        })}
      </MapContainer>
    </div>
  );
};

export default MachineMap;