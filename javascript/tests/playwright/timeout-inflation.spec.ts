import { test, expect, type Page } from "@playwright/test";

/**
 * Intentionally flaky checks against https://www.time.gov/ that demonstrate
 * timeout inflation: each timezone asserts the displayed hour is even, passing
 * immediately when it is, and parking until the test timeout when it is not.
 *
 * Pass fast / fail slow: even hours finish after page load; odd hours hang
 * until test.setTimeout fires, then Playwright retries the whole test
 * (retries: 3 → 4 attempts). Hawaii can inflate ~30s into ~120s+.
 *
 * Test timeouts on failure: Pacific 10s, Mountain 14s, Central 18s, Eastern 22s,
 * Alaska 28s, Hawaii 30s.
 */
test.describe("time.gov timeout inflation", () => {
  test.describe.configure({ retries: 3 });

  test("Pacific hour is even", async ({ page }) => {
    test.setTimeout(10_000);
    await assertZoneHourIsEven(page, /^Pacific\b/);
  });

  test("Mountain hour is even", async ({ page }) => {
    test.setTimeout(14_000);
    await assertZoneHourIsEven(page, /^Mountain\b/);
  });

  test("Central hour is even", async ({ page }) => {
    test.setTimeout(18_000);
    await assertZoneHourIsEven(page, /^Central\b/);
  });

  test("Eastern hour is even", async ({ page }) => {
    test.setTimeout(22_000);
    await assertZoneHourIsEven(page, /^Eastern\b/);
  });

  test("Alaska hour is even", async ({ page }) => {
    test.setTimeout(28_000);
    await assertZoneHourIsEven(page, /^Alaska\b/);
  });

  test("Hawaii hour is even", async ({ page }) => {
    test.setTimeout(30_000);
    await assertZoneHourIsEven(page, /^Hawaii\b/);
  });
});

async function assertZoneHourIsEven(
  page: Page,
  titlePattern: RegExp,
): Promise<void> {
  await page.goto("https://www.time.gov/");

  const clockBox = page.locator(".clock-box").filter({
    has: page.locator(".title", { hasText: titlePattern }),
  });
  const time = clockBox.locator("time").first();
  await expect(time).toBeVisible();

  // Pass fast: assert immediately. Fail slow: hang until test timeout.
  try {
    const timeText = (await time.innerText()).trim();
    const match = timeText.match(/^(\d{1,2}):/);
    expect(match, `unexpected time.gov clock text: ${timeText}`).not.toBeNull();
    const hour = Number(match![1]);
    expect(
      hour % 2,
      `${titlePattern} hour ${hour} from "${timeText}" should be even`,
    ).toBe(0);
  } catch {
    // force waiting - this will cause the timeout to eventually fire. simulating an inflated timeout setup
    await new Promise(() => {});
  }
}
