export default function Column({ children }) {
    return (
        <div className={`md:col-count-2 print:col-count-2 col-gap-md md:h-a4 print:h-letter-col col-fill-auto`}>
            {children}        
        </div>  
    )
}