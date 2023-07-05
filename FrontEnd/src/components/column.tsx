export default function Column({ children }) {
    return (
        <div className={`md:col-count-1 print:col-count-1 col-gap-md md:h-a4 print:h-a4`}>
            {children}        
        </div>  
    )
}