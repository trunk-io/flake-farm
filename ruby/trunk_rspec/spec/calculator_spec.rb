# spec/calculator_spec.rb
# spec_helper is automatically loaded via .rspec file
require 'ostruct'
require_relative "../lib/trunk_calculator"

describe TrunkCalculator do
  describe ".add" do
    context "given 1 and 2" do
      it "returns " do
        expect(TrunkCalculator.add(1, 2)).to eq(3)
      end
    end
  end

  describe ".halve" do
    context "given the number 4" do
      it "returns 2" do
        expect(TrunkCalculator.halve(4)).to eq(2)
      end
    end
  end

  describe ".halve" do
    context "given the number 3" do
      it "returns 1.5" do
        expect(TrunkCalculator.halve(3)).to eq(1.5)
      end
    end
  end

  # Example where RSpec inserts Ruby object information into test names
  describe ".add" do
    let(:numbers) { [1, 2, 3] }
    
    context "with numbers #{[1, 2, 3]}" do
      it "handles array #{[1, 2, 3]}" do
        result = numbers.sum
        expect(result).to eq(6)
      end
    end

    context "with hash" do
      let(:test_hash) { { a: 1, b: 2 } }
      
      it "processes #{ { a: 1, b: 2 } }" do
        expect(test_hash.values.sum).to eq(3)
      end
    end

    # Using subject with object interpolation
    describe "with custom object" do
      let(:custom_obj) { OpenStruct.new(value: 42) }
      
      subject { TrunkCalculator.add(custom_obj.value, 1) }
      
      context "when object is #{OpenStruct.new(value: 42)}" do
        it { is_expected.to eq(43) }
      end
    end
  end
end
