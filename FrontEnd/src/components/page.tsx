export default function Page({ children }) {
    return (
        <div className="p-6 mx-auto page max-w-2xl print:max-w-a4 md:max-w-a4 md:h-a4 xsm:p-8 sm:p-9 md:p-16 bg-white">
            {children}
        </div>
    )
}