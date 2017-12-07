class ChargesController < ApplicationController
  def create

    # Get the donation amount in cents
    @amount = params[:amount]

    @amount = @amount.gsub('$', '').gsub(',', '')

    begin
      @amount = Float(@amount).round(2)
    rescue
      flash[:error] = 'Charge not completed. Please enter a valid amount in USD ($).'
      redirect_to new_charge_path
      return
    end

    @amount = (@amount * 100).to_i # Must be an integer!

    if @amount < 500
      flash[:error] = 'Charge not completed. Donation amount must be at least $5.'
      redirect_to new_charge_path
      return
    end



    #create the customer object
    customer = Stripe::Customer.create(
        :email => params[:stripeEmail],
        :source  => params[:stripeToken]
    )

    #charge the card
    #create the charge object
    charge = Stripe::Charge.create(
        :customer    => customer.id,
        :amount => @amount,
        :currency    => 'cad',
        :description => 'custom donation',
        :source => params[:stripeToken]

    )
    charge = Stripe::Charge.retrieve("ch_1A9eP02eZvKYlo2CkibleoVM")
    charge.capture

    #get paid
    #create the payout object
    payout = Stripe::Payout.create(
        :amount => @amount,
        :currency => "cad",
    )

    #create purchase object
    # to keep track of purchases
    purchase = Purchase.create(email: params[:stripeEmail], card: params[:stripeToken],
                               amount: @amount, description: product.description, currency: "cad",
                               customer_id: customer.id, product_id: product.id, uuid: SecureRandom.uuid)
    redirect_to purchase


  rescue Stripe::CardError => e
    flash[:error] = e.message
    redirect_to new_charge_path
  end
end