describe('resume site', () => {
  Cypress.config('defaultCommandTimeout', 30000);

  it('displays total views', () => {
    cy.visit("/");

    cy.get('#total-views').invoke('text').then(parseFloat).should('be.gt', 0);
  }),
  it('increments total views', () => {
    cy.visit("/");

    cy.get('#total-views').invoke('text').then((text) => {
      const totalViews = parseFloat(text);

      cy.visit("/");

      cy.get('#total-views').invoke('text').then(parseFloat).should('be.gt', totalViews);
    });
  })
})