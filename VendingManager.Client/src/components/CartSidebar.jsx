import { useCart } from '../context/CartContext';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Trash2, ShoppingBag, CreditCard, Plus, Minus } from 'lucide-react';

function CartSidebar({ onCheckout }) {
    const { 
        cartItems, 
        removeFromCart, 
        addToCart, 
        decreaseQuantity, 
        cartTotal, 
        isCartOpen, 
        setIsCartOpen 
    } = useCart();

    return (
        <AnimatePresence>
            {isCartOpen && (
                <>
                    <motion.div 
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        exit={{ opacity: 0 }}
                        onClick={() => setIsCartOpen(false)}
                        className="fixed inset-0 bg-black/60 backdrop-blur-sm z-40"
                    />

                    <motion.div 
                        initial={{ x: '100%' }}
                        animate={{ x: 0 }}
                        exit={{ x: '100%' }}
                        transition={{ type: 'spring', damping: 25, stiffness: 200 }}
                        className="fixed right-0 top-0 h-full w-full sm:w-[450px] bg-white dark:bg-neutral-950 shadow-2xl z-50 flex flex-col border-l dark:border-neutral-800"
                    >
                        {/* Header */}
                        <div className="p-6 border-b border-gray-100 dark:border-neutral-800 flex justify-between items-center bg-white dark:bg-neutral-950">
                            <div className="flex items-center gap-3">
                                <div className="p-2 bg-blue-50 dark:bg-neutral-800 rounded-lg text-blue-600 dark:text-blue-400">
                                    <ShoppingBag size={24} />
                                </div>
                                <div>
                                    <h2 className="text-xl font-bold text-gray-800 dark:text-white">Twój Koszyk</h2>
                                    <p className="text-xs text-gray-400 font-medium">Zarządzaj swoimi produktami</p>
                                </div>
                            </div>
                            <button 
                                onClick={() => setIsCartOpen(false)}
                                className="p-2 hover:bg-gray-100 dark:hover:bg-neutral-800 rounded-full transition-colors text-gray-500 dark:text-neutral-400"
                            >
                                <X size={24} />
                            </button>
                        </div>

                        {/* Lista produktów */}
                        <div className="flex-1 overflow-y-auto p-6 space-y-4 bg-gray-50/50 dark:bg-neutral-900">
                            {cartItems.length === 0 ? (
                                <div className="h-full flex flex-col items-center justify-center text-gray-400 dark:text-neutral-500 space-y-4">
                                    <ShoppingBag size={64} strokeWidth={1} />
                                    <p className="text-lg font-medium">Koszyk jest pusty</p>
                                    <button 
                                        onClick={() => setIsCartOpen(false)}
                                        className="text-blue-600 dark:text-blue-400 hover:text-blue-700 font-medium text-sm"
                                    >
                                        Wróć do zakupów
                                    </button>
                                </div>
                            ) : (
                                cartItems.map((item, index) => (
                                    <motion.div 
                                        layout
                                        initial={{ opacity: 0, y: 20 }}
                                        animate={{ opacity: 1, y: 0 }}
                                        key={`${item.productId}-${index}`} 
                                        className="bg-white dark:bg-neutral-900 p-4 rounded-xl shadow-sm border border-gray-100 dark:border-neutral-800 flex flex-col gap-3 group"
                                    >
                                        <div className="flex justify-between items-start">
                                            <h3 className="font-semibold text-gray-800 dark:text-gray-100 text-lg leading-tight">
                                                {item.productName}
                                            </h3>
                                            <span className="font-bold text-blue-600 dark:text-blue-400 whitespace-nowrap ml-2">
                                                {(item.count * item.price).toFixed(2)} zł
                                            </span>
                                        </div>

                                        <div className="flex justify-between items-center mt-1">
                                            <div className="text-sm text-gray-400">
                                                Cena: {item.price.toFixed(2)} zł / szt.
                                            </div>

                                            <div className="flex items-center gap-4">
                                                {/* Stepper Dark Mode */}
                                                <div className="flex items-center bg-gray-100 dark:bg-neutral-950 rounded-lg p-1 border border-gray-200 dark:border-neutral-800">
                                                    <button 
                                                        onClick={() => decreaseQuantity(item.productId, item.machineId)}
                                                        className="w-8 h-8 flex items-center justify-center bg-white dark:bg-neutral-900 rounded-md text-gray-600 dark:text-gray-300 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/30 shadow-sm transition-all disabled:opacity-50"
                                                    >
                                                        <Minus size={16} />
                                                    </button>
                                                    
                                                    <span className="w-10 text-center font-bold text-gray-700 dark:text-gray-200 select-none">
                                                        {item.count}
                                                    </span>

                                                    <button 
                                                        onClick={() => addToCart(item, item.machineId)}
                                                        className="w-8 h-8 flex items-center justify-center bg-white dark:bg-neutral-900 rounded-md text-gray-600 dark:text-gray-300 hover:text-green-600 hover:bg-green-50 dark:hover:bg-green-900/30 shadow-sm transition-all"
                                                    >
                                                        <Plus size={16} />
                                                    </button>
                                                </div>

                                                <button 
                                                    className="p-2 text-gray-300 hover:text-red-500 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-lg transition-all"
                                                    onClick={() => removeFromCart(item.productId, item.machineId)}
                                                >
                                                    <Trash2 size={20} />
                                                </button>
                                            </div>
                                        </div>
                                    </motion.div>
                                ))
                            )}
                        </div>

                        {/* Footer */}
                        <div className="p-6 border-t border-gray-100 dark:border-neutral-800 bg-white dark:bg-neutral-950 shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.05)]">
                            <div className="flex justify-between items-end mb-6">
                                <div>
                                    <p className="text-sm text-gray-500 dark:text-gray-400">Suma do zapłaty</p>
                                </div>
                                <span className="text-3xl font-bold text-gray-900 dark:text-white tracking-tight">
                                    {cartTotal.toFixed(2)} <span className="text-lg text-gray-400 font-normal">PLN</span>
                                </span>
                            </div>
                            <button 
                                className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white py-4 rounded-xl font-bold shadow-lg shadow-blue-500/30 hover:shadow-blue-500/50 hover:scale-[1.02] active:scale-[0.98] transition-all flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
                                disabled={cartItems.length === 0}
                                onClick={onCheckout}
                            >
                                <CreditCard size={20} />
                                Przejdź do płatności
                            </button>
                        </div>
                    </motion.div>
                </>
            )}
        </AnimatePresence>
    );
}

export default CartSidebar;