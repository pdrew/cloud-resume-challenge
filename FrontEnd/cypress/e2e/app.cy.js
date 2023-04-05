describe('resume site', () => {

  Cypress.config('defaultCommandTimeout', 30000);

  it('displays total views', () => {
    cy.visit('http://localhost:3000')

    cy.get('#total-views').invoke('text').then(parseFloat).should('be.gt', 0)
  }),
  it('increments total views', () => {
    cy.visit('http://localhost:3000')

    cy.get('#total-views').invoke('text').then((text) => {
      const totalViews = parseFloat(text)

      cy.visit('http://localhost:3000')

      cy.get('#total-views').invoke('text').then(parseFloat).should('be.gt', totalViews)
    })
  })
})