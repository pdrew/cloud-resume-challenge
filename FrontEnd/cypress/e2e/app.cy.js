describe('resume site', () => {

  const url = Cypress.env('RESUME_SITE_URL');

  Cypress.config('defaultCommandTimeout', 30000);

  it('displays total views', () => {
    cy.visit(url);

    cy.get('#total-views').invoke('text').then(parseFloat).should('be.gt', 0);
  }),
  it('increments total views', () => {
    cy.visit(url);

    cy.get('#total-views').invoke('text').then((text) => {
      const totalViews = parseFloat(text);

      cy.visit(url);

      cy.get('#total-views').invoke('text').then(parseFloat).should('be.gt', totalViews);
    });
  })
})