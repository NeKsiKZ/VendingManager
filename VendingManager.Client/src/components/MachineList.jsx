import React from 'react';
import { motion, AnimatePresence } from "framer-motion";
import { Monitor, MapPin, Search } from "lucide-react";
import MachineSkeleton from './MachineSkeleton';

const cardVariants = {
  hidden: { opacity: 0, y: 20, scale: 0.95 },
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
  exit: { opacity: 0, scale: 0.9, transition: { duration: 0.2 } },
};

const MachineList = ({ machines, loading, onMachineSelect }) => {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 relative z-10 min-h-[50vh] content-start">
      {loading ? (
        Array(6).fill(0).map((_, i) => <MachineSkeleton key={i} />)
      ) : (
        <AnimatePresence mode="popLayout">
          {machines.length > 0 ? (
            machines.map((machine, index) => (
              <motion.div
                layout="position"
                custom={index}
                variants={cardVariants}
                initial="hidden"
                animate="visible"
                exit="exit"
                whileHover={{ y: -8, transition: { type: "spring", stiffness: 300, damping: 15 } }}
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
                  onClick={() => onMachineSelect(machine)}
                >
                  Zobacz asortyment
                </button>
              </motion.div>
            ))
          ) : (
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
  );
};

export default MachineList;