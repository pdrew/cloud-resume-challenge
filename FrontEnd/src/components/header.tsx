
export default function Header() {
    return (
        <header className="flex items-center mb-8 md:mb-11">
        <div   className="initials-container mr-5 text-base leading-none text-white bg-gray-700 font-medium print:bg-black px-3"
          style={{paddingBottom: "0.6875rem", paddingTop: "0.6875rem"}}>
          <div className="initial text-center" style={{ paddingBottom: "0.1875rem" }}>P</div>
          <div className="text-center initial">D</div>
        </div>
        <h1 className="text-2xl font-semibold text-gray-750 pb-px">
          Patrick Drew
        </h1>
      </header>
    )
}