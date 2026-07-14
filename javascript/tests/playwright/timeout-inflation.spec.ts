import { test, expect, type Page } from "@playwright/test";

/**
 * Intentionally slow, flaky checks against https://www.time.gov/ that demonstrate
 * timeout inflation: each timezone waits a different fixed delay before asserting
 * that the displayed hour is even, then retries 3 times on failure.
 *
 * Worst-case wall time per attempt ≈ wait + page load; with retries: 3 the
 * failing case is attempted 4 times total, so Hawaii can inflate ~40s into ~160s+.
 *
 * Wait schedule (seconds): Pacific 10, Mountain 20, Central 25, Eastern 30,
 * Alaska 35, Hawaii 40.
 */
test.describe("time.gov timeout inflation", () => {
  test.describe.configure({ retries: 3 });

  test("Pacific hour is even", async ({ page }) => {
    test.setTimeout(10_000);
    await assertZoneHourIsEven(page, {
      titlePattern: /^Pacific\b/,
      waitMs: 8_000,
    });
  });

  test("Mountain hour is even", async ({ page }) => {
    test.setTimeout(14_000);
    await assertZoneHourIsEven(page, {
      titlePattern: /^Mountain\b/,
      waitMs: 12_000,
    });
  });

  test("Central hour is even", async ({ page }) => {
    test.setTimeout(18_000);
    await assertZoneHourIsEven(page, {
      titlePattern: /^Central\b/,
      waitMs: 16_000,
    });
  });

  test("Eastern hour is even", async ({ page }) => {
    test.setTimeout(22_000);
    await assertZoneHourIsEven(page, {
      titlePattern: /^Eastern\b/,
      waitMs: 20_000,
    });
  });

  test("Alaska hour is even", async ({ page }) => {
    test.setTimeout(28_000);
    await assertZoneHourIsEven(page, {
      titlePattern: /^Alaska\b/,
      waitMs: 24_000,
    });
  });

  test("Hawaii hour is even", async ({ page }) => {
    test.setTimeout(30_000);
    await assertZoneHourIsEven(page, {
      titlePattern: /^Hawaii\b/,
      waitMs: 28_000,
    });
  });
});

async function assertZoneHourIsEven(
  page: Page,
  options: { titlePattern: RegExp; waitMs: number },
): Promise<void> {
  await page.goto("https://www.time.gov/");

  const clockBox = page.locator(".clock-box").filter({
    has: page.locator(".title", { hasText: options.titlePattern }),
  });
  const time = clockBox.locator("time").first();
  await expect(time).toBeVisible();

  // Fixed delay before the assertion — this is what inflates under retries.
  await page.waitForTimeout(options.waitMs);

  const timeText = (await time.innerText()).trim();
  const match = timeText.match(/^(\d{1,2}):/);
  expect(match, `unexpected time.gov clock text: ${timeText}`).not.toBeNull();

  const hour = Number(match![1]);
  expect(
    hour % 2,
    `${options.titlePattern} hour ${hour} from "${timeText}" should be even`,
  ).toBe(0);
}
