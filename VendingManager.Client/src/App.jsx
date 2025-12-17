import { useState, useEffect } from "react";
import { CartProvider, useCart } from "./context/CartContext";
import { useTheme } from "./context/ThemeContext";
import CartSidebar from "./components/CartSidebar";
import Navbar from "./components/Navbar";
import MachineMap from "./components/MachineMap";
import MachineList from "./components/MachineList";
import ProductModal from "./components/ProductModal";
import {
  Wifi,
  Monitor,
  ShoppingCart,
  Sun,
  Moon,
} from "lucide-react";

export default function AppWrapper() {
  return (
    <CartProvider>
      <App />
    </CartProvider>
  );
}

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
          const response = await fetch(`${API_URL}/${item.machineId}/sale`, {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              "X-API-KEY": API_KEY,
            },
            body: JSON.stringify({ productId: item.productId }),
          });

          if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Błąd zakupu");
          }
        }
      }
      showNotification("Zakup zakończony sukcesem!", "success");
      clearCart();
      if (selectedMachine) openMachineModal(selectedMachine);
    } catch (e) {
      showNotification(e.message || "Wystąpił błąd podczas zakupu.", "error");
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

      {/* Powiadomienia (Toast) */}
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

      <Navbar
        searchTerm={searchTerm}
        setSearchTerm={setSearchTerm}
        statusFilter={statusFilter}
        setStatusFilter={setStatusFilter}
      />

      <div className="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8 space-y-8">
        {/* Mapa */}
        <MachineMap
          machines={filteredMachines}
          isDarkMode={isDarkMode}
          onMachineSelect={openMachineModal}
        />

        {/* Lista Automatów */}
        <MachineList
          machines={filteredMachines}
          loading={loading}
          onMachineSelect={openMachineModal}
        />
      </div>

      {/* Modal Produktów */}
      <ProductModal
        machine={selectedMachine}
        products={products}
        loading={productsLoading}
        onClose={() => setSelectedMachine(null)}
        onAddToCart={(product) => addToCart(product, selectedMachine.id)}
      />

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