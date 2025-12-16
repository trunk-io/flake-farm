import { test, expect } from '@playwright/test';

test.describe('Typos Page Tests', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('https://the-internet.herokuapp.com/typos');
  });

  test('should find the correct text without typo', async ({ page }) => {
    // This test will be flaky because the typo appears randomly
    // Sometimes the text says "won,t" (with typo), other times "won't" (correct)
    const content = await page.textContent('body');
    expect(content).toContain("won't");
    expect(content).not.toContain("won,t");
  });

  test('should verify page title', async ({ page }) => {
    await expect(page).toHaveTitle(/The Internet/);
  });

  test('should check for typos in the main content', async ({ page }) => {
    // Get the paragraph text that may or may not have a typo
    const paragraph = page.locator('p').filter({ hasText: /Sometimes you'll see a typo/ });
    const text = await paragraph.textContent();
    
    // This assertion will fail randomly when the typo appears
    expect(text).toBe("Sometimes you'll see a typo, other times you won't.");
  });

  test('should reload and check for typo consistency', async ({ page }) => {
    // First load
    const firstLoad = await page.locator('p').filter({ hasText: /Sometimes you'll see a typo/ }).textContent();
    
    // Reload the page (may or may not show the typo)
    await page.reload();
    
    const secondLoad = await page.locator('p').filter({ hasText: /Sometimes you'll see a typo/ }).textContent();
    
    // This will fail randomly since the typo is random on each load
    expect(firstLoad).toBe(secondLoad);
  });

  test('should verify correct spelling after multiple reloads', async ({ page }) => {
    // Try multiple times to catch the typo
    let foundTypo = false;
    for (let i = 0; i < 5; i++) {
      await page.reload();
      const text = await page.locator('p').filter({ hasText: /Sometimes you'll see a typo/ }).textContent();
      if (text?.includes("won,t")) {
        foundTypo = true;
        break;
      }
    }
    
    // This test expects to never find a typo, but it will randomly fail
    expect(foundTypo).toBe(false);
  });
});

