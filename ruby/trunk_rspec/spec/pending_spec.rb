# spec/pending_spec.rb
# Exercises how the trunk rspec plugin reports pending/skip examples.
# RSpec inverts a pending example's outcome, so the reported status is:
#   skip / xit                  -> skipped
#   pending, body still failing -> success
#   pending, body now passing   -> failure (PendingExampleFixedError)
require_relative "../lib/calculator"

describe "TrunkCalculator pending and skip handling" do
  it "reports an explicit skip as skipped" do
    skip("not ready yet")
    expect(TrunkCalculator.add(1, 1)).to eq(3)
  end

  xit "reports xit as skipped" do
    expect(TrunkCalculator.add(1, 1)).to eq(3)
  end

  it "reports a still-broken pending example as success" do
    pending("halve truncates instead of returning a float")
    expect(TrunkCalculator.halve(3)).to eq(1.5)
  end

  it "reports a fixed pending example as failure" do
    pending("expected to still fail, but the body now passes")
    expect(TrunkCalculator.add(1, 2)).to eq(3)
  end
end
