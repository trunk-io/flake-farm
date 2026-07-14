import { test, expect } from "@playwright/test";

/**
 * These tests are intentionally broken so the Flake Farm produces JUnit output
 * that exercises Trunk's flaky-test detection. Retries are forced on here so the
 * failing cases below are attempted multiple times regardless of the CI env, and
 * the reporter is configured with `includeRetries: true` so every attempt shows
 * up in tests/playwright/junitresults-playwright.xml.
 */
test.describe("playwright retries", () => {
  // Force retries even when not running under CI so we always emit retry data.
  test.describe.configure({ retries: 2 });

  test("fails on every retry", async ({ page }) => {
    await page.goto("https://playwright.dev/");

    // Assertion failure ("Playwroght" is a typo): reported as a JUnit <failure>.
    // Retried twice and still fails, producing retry entries in the XML.
    await expect(page).toHaveTitle("Playwroght");
  });

  test("flakes intermittently across retries", async ({ page }) => {
    await page.goto("https://playwright.dev/");

    // Time-based coin flip: passes or fails depending on the wall clock, so the
    // outcome can differ between the initial attempt and its retries.
    expect(Date.now() % 2).toBe(0);
  });
});

/**
 * Both of the tests below come from the same technology (Playwright) but land in
 * JUnit as different result types: one as a <failure> (a failed assertion) and
 * one as an <error> (an uncaught exception thrown outside of an expect).
 */
test.describe("playwright error vs failure", () => {
  test.describe.configure({ retries: 2 });

  test("failure: assertion does not hold", async ({ page }) => {
    await page.goto("https://playwright.dev/");

    // Assertion failure -> JUnit <failure>.
    await expect(
      page.getByRole("heading", { name: "Nonexistent Heading" }),
    ).toBeVisible({ timeout: 2000 });
  });

  test("error: throws an uncaught exception", async () => {
    // Thrown exception (not an expect assertion) -> JUnit <error>.
    // JSON.parse throws a SyntaxError before any expect() runs.
    const data = JSON.parse("{ this is not valid json }");
    expect(data).toBeTruthy();
  });
});
