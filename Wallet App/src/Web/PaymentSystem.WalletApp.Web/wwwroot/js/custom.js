function getTransactionInfo(id) {
    console.log(id);

    const uri = `/transactions/getTransactionDetails?id=${id}`;

    const options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        }
    };

    fetch(uri, options)
        .then(res => res.json())
        .then(res => {
            const totalAmountValue = res.amount + res.fee;

            const totalAmountHeader = document.getElementById('total-amount-header');
            totalAmountHeader.textContent = `${totalAmountValue.toFixed(2)}P`;

            const dateHeader = document.getElementById('date-header');
            dateHeader.textContent = res.date;

            const paymentAmount = document.getElementById('payment-amount');
            paymentAmount.textContent = `${res.amount.toFixed(2)}P`;

            const transactionFee = document.getElementById('transaction-fee');
            transactionFee.textContent = `${res.fee.toFixed(2)}P`;

            const totalAmount = document.getElementById('total-amount');
            totalAmount.textContent = `${totalAmountValue.toFixed(2)}P`;

            const senderAddress = document.getElementById('sender-address');
            senderAddress.textContent = res.sender;

            const recipientAddress = document.getElementById('recipient-address');
            recipientAddress.textContent = res.recipient;

            const transactionHash = document.getElementById('transaction-hash');
            transactionHash.textContent = res.hash;

            const blockHash = document.getElementById('block-hash');
            blockHash.textContent = res.blockHash;
        });
}
