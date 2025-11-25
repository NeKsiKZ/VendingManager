import { useState, useEffect } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import { CartProvider, useCart } from "./context/CartContext";
import { useTheme } from "./context/ThemeContext";
import CartSidebar from "./components/CartSidebar";
import {
  Search,
  MapPin,
  Monitor,
  Wifi,
  ShoppingCart,
  Plus,
  X,
  Sun,
  Moon,
  Loader2,
} from "lucide-react";
import { renderToStaticMarkup } from "react-dom/server";
import { motion, AnimatePresence } from "framer-motion";

// --- KONFIGURACJA IKON MAPY ---
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

const cardVariants = {
  hidden: {
    opacity: 0,
    y: 20,
    scale: 0.95,
  },
  visible: (i) => ({
    opacity: 1,
    y: 0,
    scale: 1,
    transition: {
      delay: i * 0.1,
      duration: 0.4,
      type: "spring",
      stiffness: 100,
      damping: 15,
    },
  }),
  exit: {
    opacity: 0,
    scale: 0.9,
    transition: { duration: 0.2 },
  },
};

export default function AppWrapper() {
  return (
    <CartProvider>
      <App />
    </CartProvider>
  );
}

// --- KOMPONENT SZKIELETU ---
const MachineSkeleton = () => (
  <div className="bg-white dark:bg-neutral-900 rounded-2xl p-6 shadow-sm border border-gray-100 dark:border-neutral-800 flex flex-col h-full animate-pulse">
    <div className="flex justify-between items-start mb-4">
      <div className="w-12 h-12 bg-gray-200 dark:bg-neutral-800 rounded-xl"></div>
      <div className="w-16 h-6 bg-gray-200 dark:bg-neutral-800 rounded-full"></div>
    </div>
    <div className="h-6 bg-gray-200 dark:bg-neutral-800 rounded w-3/4 mb-2"></div>
    <div className="h-4 bg-gray-200 dark:bg-neutral-800 rounded w-1/2 mb-6"></div>
    <div className="mt-auto h-12 bg-gray-200 dark:bg-neutral-800 rounded-xl w-full"></div>
  </div>
);

function App() {
  const [machines, setMachines] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [selectedMachine, setSelectedMachine] = useState(null);
  const [products, setProducts] = useState([]);
  const [productsLoading, setProductsLoading] = useState(false);
  const [notification, setNotification] = useState(null);

  const { addToCart, cartItems, clearCart, setIsCartOpen, cartCount } =
    useCart();
  const { isDarkMode, toggleTheme } = useTheme();

  const API_URL = import.meta.env.VITE_API_URL;
  const API_KEY = import.meta.env.VITE_API_KEY;

  useEffect(() => {
    fetchMachines();
  }, []);

  const fetchMachines = () => {
    setLoading(true);
    if (!API_URL) {
      console.error("BŁĄD: Brak VITE_API_URL w pliku .env");
      setLoading(false);
      return;
    }

    fetch(`${API_URL}?pageSize=100`, { headers: { "X-API-KEY": API_KEY } })
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP error! Status: ${res.status}`);
        return res.json();
      })
      .then((data) => {
        const machinesList = data.items || data;
        setMachines(machinesList);
        setTimeout(() => setLoading(false), 500);
      })
      .catch((err) => {
        console.error("Błąd pobierania maszyn:", err);
        setLoading(false);
      });
  };

  const openMachineModal = (machine) => {
    setSelectedMachine(machine);
    setProductsLoading(true);
    fetch(`${API_URL}/${machine.id}/inventory`, {
      headers: { "X-API-KEY": API_KEY },
    })
      .then((res) => res.json())
      .then((data) => {
        setProducts(data);
        setProductsLoading(false);
      })
      .catch((err) => {
        console.error("Błąd inventory:", err);
        setProductsLoading(false);
      });
  };

  const handleCheckout = async () => {
    if (!confirm(`Czy chcesz kupić ${cartCount} produktów?`)) return;

    setIsCartOpen(false);
    showNotification("Przetwarzanie płatności...", "info");

    try {
      for (const item of cartItems) {
        for (let i = 0; i < item.count; i++) {
          await fetch(`${API_URL}/${item.machineId}/sale`, {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              "X-API-KEY": API_KEY,
            },
            body: JSON.stringify({ productId: item.productId }),
          });
        }
      }
      showNotification("Zakup zakończony sukcesem!", "success");
      clearCart();
      if (selectedMachine) openMachineModal(selectedMachine);
    } catch (e) {
      showNotification("Wystąpił błąd podczas zakupu.", "error");
    }
  };

  const showNotification = (msg, type) => {
    setNotification({ msg, type });
    setTimeout(() => setNotification(null), 3000);
  };

  const filteredMachines = machines.filter((machine) => {
    const name = machine.name || "";
    const location = machine.location || "";

    const matchesSearch =
      name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      location.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus =
      statusFilter === "All" || machine.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-neutral-950 text-neutral-800 dark:text-neutral-200 font-sans transition-colors duration-300 relative">
      {/* TŁO AMBIENT */}
      <div className="fixed inset-0 z-0 pointer-events-none overflow-hidden">
        <div className="absolute top-[-10%] left-[-10%] w-[500px] h-[500px] bg-blue-500/20 dark:bg-blue-600/10 rounded-full blur-[100px] animate-pulse"></div>
        <div className="absolute top-[20%] right-[-5%] w-[400px] h-[400px] bg-indigo-500/20 dark:bg-indigo-600/10 rounded-full blur-[120px]"></div>
      </div>

      {/* Przełącznik Dark Mode */}
      <button
        onClick={toggleTheme}
        className="fixed top-6 left-6 z-30 p-3 rounded-full bg-white dark:bg-neutral-800 shadow-lg text-yellow-500 dark:text-blue-400 hover:scale-110 transition-transform border border-gray-100 dark:border-neutral-700"
      >
        {isDarkMode ? <Moon size={24} /> : <Sun size={24} />}
      </button>

      {/* Koszyk Button */}
      <button
        className="fixed bottom-8 right-8 z-30 w-16 h-16 bg-gradient-to-r from-blue-600 to-indigo-600 rounded-full shadow-lg shadow-blue-500/40 hover:scale-110 hover:shadow-blue-500/60 transition-all flex items-center justify-center group"
        onClick={() => setIsCartOpen(true)}
      >
        <ShoppingCart
          className="text-white group-hover:rotate-12 transition-transform"
          size={28}
        />
        {cartCount > 0 && (
          <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs font-bold w-6 h-6 flex items-center justify-center rounded-full border-2 border-white dark:border-neutral-900 shadow-sm">
            {cartCount}
          </span>
        )}
      </button>

      <CartSidebar onCheckout={handleCheckout} />

      {/* Powiadomienia */}
      {notification && (
        <div
          className={`fixed top-6 right-6 z-50 px-6 py-4 rounded-xl shadow-2xl animate-bounce-in text-white font-medium flex items-center gap-3 ${
            notification.type === "success"
              ? "bg-green-600"
              : notification.type === "error"
              ? "bg-red-600"
              : "bg-blue-600"
          }`}
        >
          {notification.type === "success" ? (
            <Wifi size={20} />
          ) : (
            <Monitor size={20} />
          )}
          {notification.msg}
        </div>
      )}

      {/* Hero Header */}
      <div className="bg-white dark:bg-neutral-950/80 backdrop-blur-md border-b border-gray-200 dark:border-neutral-900 sticky top-0 z-20 transition-colors duration-300">
        <div className="max-w-7xl mx-auto px-4 py-6 sm:px-6 lg:px-8">
          <div className="flex flex-col xl:flex-row xl:items-center gap-6 justify-between">
            {/* Tytuł i Opis */}
            <div className="text-center xl:text-left min-w-[250px]">
              <h1 className="text-3xl md:text-4xl font-extrabold tracking-tight text-transparent bg-clip-text bg-gradient-to-r from-blue-600 to-indigo-600 pb-1">
                VendingManager
              </h1>
              <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                Znajdź najbliższy automat i kup napój.
              </p>
            </div>

            {/* Pasek Wyszukiwania i Filtry */}
            <div className="flex flex-col md:flex-row gap-3 w-full xl:max-w-4xl">
              {/* Search Bar */}
              <div className="relative w-full md:flex-1 shadow-sm shadow-gray-200/50 dark:shadow-black/40 rounded-xl">
                <Search
                  className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"
                  size={20}
                />
                <input
                  type="text"
                  className="w-full pl-11 pr-4 py-3 rounded-xl bg-gray-50 dark:bg-neutral-900 border border-gray-100 dark:border-neutral-800 focus:border-blue-500 dark:focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 outline-none transition-all placeholder-gray-400 dark:text-white text-base"
                  placeholder="Szukaj automatu..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>

              {/* Filtry */}
              <div className="grid grid-cols-3 gap-2 w-full md:w-auto md:flex md:justify-start shrink-0">
                {[
                  { value: "All", label: "Wszystkie", icon: null },
                  {
                    value: "Online",
                    label: "Online",
                    icon: (
                      <div className="w-2 h-2 rounded-full bg-green-500 animate-pulse mr-2" />
                    ),
                  },
                  {
                    value: "Offline",
                    label: "Offline",
                    icon: (
                      <div className="w-2 h-2 rounded-full bg-red-500 mr-2" />
                    ),
                  },
                ].map((status) => (
                  <button
                    key={status.value}
                    onClick={() => setStatusFilter(status.value)}
                    className={`
                                    flex items-center justify-center px-2 sm:px-4 py-3 rounded-xl font-medium text-xs sm:text-sm transition-all duration-300 border whitespace-nowrap w-full md:w-auto
                                    ${
                                      statusFilter === status.value
                                        ? "bg-neutral-900 dark:bg-white text-white dark:text-neutral-900 border-transparent shadow-md scale-[1.02]"
                                        : "bg-gray-50 dark:bg-neutral-900 text-gray-600 dark:text-gray-400 border-gray-100 dark:border-neutral-800 hover:bg-gray-100 dark:hover:bg-neutral-800"
                                    }
                                `}
                  >
                    {status.icon}
                    {status.label}
                  </button>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8 space-y-8">
        {/* Mapa */}
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
            {filteredMachines.map((m) => {
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
                        onClick={() => openMachineModal(m)}
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

        {/* GRID AUTOMATÓW */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 relative z-10 min-h-[50vh] content-start">
          {loading ? (
            Array(6)
              .fill(0)
              .map((_, i) => <MachineSkeleton key={i} />)
          ) : (
            <AnimatePresence mode="popLayout">
              {filteredMachines.length > 0 ? (
                filteredMachines.map((machine, index) => (
                  <motion.div
                    layout="position"
                    custom={index}
                    variants={cardVariants}
                    initial="hidden"
                    animate="visible"
                    exit="exit"
                    whileHover={{
                      y: -8,
                      transition: {
                        type: "spring",
                        stiffness: 300,
                        damping: 15,
                      },
                    }}
                    key={machine.id}
                    className="group bg-white dark:bg-neutral-900 rounded-2xl p-6 shadow-sm border border-gray-100 dark:border-neutral-800 hover:shadow-2xl hover:border-blue-200 dark:hover:border-blue-900/50 flex flex-col h-full"
                  >
                    <div className="flex justify-between items-start mb-4">
                      <div className="p-3 bg-blue-50 dark:bg-neutral-800 rounded-xl text-blue-600 dark:text-blue-400 group-hover:bg-blue-600 group-hover:text-white transition-colors duration-300">
                        <Monitor size={24} />
                      </div>
                      <span
                        className={`px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider ${
                          machine.status === "Online"
                            ? "bg-green-100 text-green-700 dark:bg-green-900/20 dark:text-green-400"
                            : "bg-red-100 text-red-700 dark:bg-red-900/20 dark:text-red-400"
                        }`}
                      >
                        {machine.status}
                      </span>
                    </div>

                    <h3 className="text-xl font-bold text-gray-800 dark:text-white mb-2">
                      {machine.name}
                    </h3>
                    <div className="flex items-center text-gray-500 dark:text-neutral-400 text-sm mb-6">
                      <MapPin size={16} className="mr-1" />
                      {machine.location}
                    </div>

                    <button
                      className="mt-auto w-full py-3 rounded-xl border-2 border-blue-600 text-blue-600 dark:border-blue-500 dark:text-blue-400 font-bold hover:bg-blue-600 hover:text-white dark:hover:bg-blue-500 dark:hover:text-white transition-colors duration-300"
                      onClick={() => openMachineModal(machine)}
                    >
                      Zobacz asortyment
                    </button>
                  </motion.div>
                ))
              ) : (
                // STAN PUSTY
                <motion.div
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  exit={{ opacity: 0 }}
                  className="col-span-full text-center py-12"
                >
                  <div className="inline-block p-4 bg-gray-100 dark:bg-neutral-800 rounded-full mb-4">
                    <Search size={40} className="text-gray-400" />
                  </div>
                  <h3 className="text-xl font-bold text-gray-700 dark:text-neutral-300">
                    Nie znaleziono automatów
                  </h3>
                  <p className="text-gray-500 mt-2">
                    Sprawdź filtry lub zmień frazę wyszukiwania.
                  </p>
                </motion.div>
              )}
            </AnimatePresence>
          )}
        </div>
      </div>

      {/* MODAL */}
      {selectedMachine && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          <div
            className="absolute inset-0 bg-black/60 backdrop-blur-sm"
            onClick={() => setSelectedMachine(null)}
          ></div>

          <div className="relative bg-white dark:bg-neutral-900 rounded-3xl shadow-2xl w-full max-w-4xl max-h-[90vh] flex flex-col overflow-hidden animate-in fade-in zoom-in duration-300 border dark:border-neutral-800">
            <div className="p-6 border-b border-gray-100 dark:border-neutral-800 flex justify-between items-center bg-gray-50 dark:bg-neutral-900">
              <div>
                <h3 className="text-2xl font-bold text-gray-800 dark:text-white">
                  {selectedMachine.name}
                </h3>
                <div className="flex items-center gap-2 mt-1">
                  <span
                    className={`w-2.5 h-2.5 rounded-full ${
                      selectedMachine.status === "Online"
                        ? "bg-green-500"
                        : "bg-red-500"
                    }`}
                  ></span>
                  <span className="text-sm text-gray-500 dark:text-neutral-400">
                    {selectedMachine.status} • {selectedMachine.location}
                  </span>
                </div>
              </div>
              <button
                onClick={() => setSelectedMachine(null)}
                className="p-2 hover:bg-gray-200 dark:hover:bg-neutral-800 rounded-full transition-colors text-gray-500 dark:text-neutral-400"
              >
                <X size={24} />
              </button>
            </div>

            <div className="flex-1 overflow-y-auto p-6 bg-gray-50/30 dark:bg-neutral-950">
              {productsLoading ? (
                <div className="flex flex-col items-center justify-center py-20 text-blue-600 dark:text-blue-400">
                  <Loader2 className="animate-spin mb-4" size={40} />
                  <p>Ładowanie produktów...</p>
                </div>
              ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                  {products.map((p) => (
                    <div
                      key={p.productId}
                      className="bg-white dark:bg-neutral-900 rounded-xl p-4 shadow-sm border border-gray-100 dark:border-neutral-800 hover:shadow-md transition-shadow flex gap-4 items-center"
                    >
                      <div className="w-20 h-20 bg-gray-100 dark:bg-neutral-800 rounded-lg flex items-center justify-center overflow-hidden flex-shrink-0 group">
                        {p.imageUrl ? (
                          <img
                            src={p.imageUrl}
                            alt={p.productName}
                            className="w-full h-full object-cover img-zoom"
                          />
                        ) : (
                          <span className="text-xs text-gray-400 text-center px-1">
                            Brak zdjęcia
                          </span>
                        )}
                      </div>
                      <div className="flex-1 min-w-0">
                        <h4 className="font-bold text-gray-800 dark:text-gray-100 truncate">
                          {p.productName}
                        </h4>
                        <div className="flex items-baseline gap-2 mb-2">
                          <span className="text-lg font-bold text-blue-600 dark:text-blue-400">
                            {p.price.toFixed(2)} zł
                          </span>
                          <span className="text-xs text-gray-400">/ szt.</span>
                        </div>

                        {p.quantity > 0 ? (
                          <button
                            onClick={() => addToCart(p, selectedMachine.id)}
                            className="w-full bg-gray-900 dark:bg-blue-600 text-white text-sm py-2 rounded-lg hover:bg-blue-600 dark:hover:bg-blue-500 transition-colors flex items-center justify-center gap-2"
                          >
                            <Plus size={16} /> Dodaj
                          </button>
                        ) : (
                          <button
                            disabled
                            className="w-full bg-gray-100 dark:bg-neutral-800 text-gray-400 text-sm py-2 rounded-lg cursor-not-allowed"
                          >
                            Wyprzedane
                          </button>
                        )}
                        <div className="mt-1 text-right">
                          <span
                            className={`text-[10px] uppercase font-bold tracking-wider ${
                              p.quantity < 5
                                ? "text-red-500 dark:text-red-400"
                                : "text-green-500 dark:text-green-400"
                            }`}
                          >
                            {p.quantity} szt.
                          </span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* STOPKA */}
      <footer className="relative z-10 bg-white dark:bg-neutral-900 border-t border-gray-200 dark:border-neutral-800 mt-20">
        <div className="max-w-7xl mx-auto px-4 py-12 sm:px-6 lg:px-8">
          <div className="flex flex-col md:flex-row justify-between items-center gap-6">
            <div className="text-center md:text-left">
              <h2 className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-blue-600 to-indigo-600">
                VendingManager
              </h2>
              <p className="text-gray-500 dark:text-gray-400 mt-2 text-sm">
                Innowacyjne zarządzanie automatami vendingowymi.
              </p>
            </div>

            <div className="flex gap-6 text-sm font-medium text-gray-500 dark:text-gray-400">
              <a
                href="#"
                className="hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
              >
                O nas
              </a>
              <a
                href="#"
                className="hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
              >
                Kontakt
              </a>
              <a
                href="#"
                className="hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
              >
                Prywatność
              </a>
            </div>

            <div className="text-xs text-gray-400 dark:text-neutral-600">
              &copy; 2025 VendingManager. Wszelkie prawa zastrzeżone. Bartosz Zimnoch
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
}
